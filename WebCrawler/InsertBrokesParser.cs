using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;

namespace WebCrawler
{
    public class InsertBrokesParser : BaseParser
    {
        public async Task RunAsync(string stockId, string name)
        {
            stockId = "1101";
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new StockDbContext();
            var brokers = context.Broker.ToList();

            var startDate = "2019-9-1";
            var endDate = "2021-1-4";

            var ss = Stopwatch.StartNew();
            ss.Start();

            var tasks = new List<Task>();
            foreach (var item in mapper)
            {
                int start = (item.Key - 1) * brokers.Count / mapper.Count;
                int end = item.Key * brokers.Count / mapper.Count;
                var _brokers = brokers.ToArray()[start..(end - 1)];
                var task = Task.Run(() => RunByUrlAsync(stockId, name, item.Value, startDate, endDate, _brokers));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
            ss.Stop();
            Console.WriteLine($"Elapsed: {ss.Elapsed.TotalSeconds}");
        }

        private Dictionary<int, string> mapper = new Dictionary<int, string>
        {
            { 1, "https://djinfo.cathaysec.com.tw"},
            { 2, "https://moneydj.emega.com.tw" },
            { 3, "https://concords.moneydj.com"},
            { 4, "https://tv.moneydj.com" },
            { 5, "http://just2.entrust.com.tw"},
            { 6, "http://stockchannel.sinotrade.com.tw" },
            { 7, "http://jsjustweb.jihsun.com.tw"},
            { 8, "http://5850web.moneydj.com" },
            { 9, "http://sod.nsc.com.tw"},
            { 10, "http://jdata.yuanta.com.tw" },
            { 11, "https://fubon-ebrokerdj.fbs.com.tw"}
        };

        private async Task RunByUrlAsync(string stockId, string name, string domain, string startDate, string endDate, Broker[] brokers)
        {
            var context = new StockDbContext();
            var details = new List<BrokerTransactionDetails>();

            foreach (var broker in brokers)
            {
                try
                {
                    var bhid = broker.BHID;
                    var b = broker.b;

                    Console.WriteLine($"{domain} {stockId} {broker.BrokerName} {broker.BHID} {broker.BrokerName}");

                    var url = $@"{domain}/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={bhid}&b={b}&C=1&D={startDate}&E={endDate}&ver=V3";

                    var rootNode = GetRootNoteByUrl(url, false);
                    var htmlNode = rootNode.SelectSingleNode("//*[@id=\"oMainTable\"]");

                    if (htmlNode == null)
                        continue;

                    for (int j = 3; j < htmlNode.ChildNodes.Count; j += 2)
                    {
                        var date = Convert.ToDateTime(htmlNode.ChildNodes[j].ChildNodes[1].InnerHtml);
                        var buy = int.Parse(htmlNode.ChildNodes[j].ChildNodes[3].InnerHtml.Replace(",", ""));
                        var sell = int.Parse(htmlNode.ChildNodes[j].ChildNodes[5].InnerHtml.Replace(",", ""));
                        var 買賣超 = int.Parse(htmlNode.ChildNodes[j].ChildNodes[9].InnerHtml.Replace(",", ""));

                        var dd = new BrokerTransactionDetails
                        {
                            BrokerId = broker.b,
                            BrokerName = broker.BrokerName,
                            StockId = stockId,
                            StockName = name,
                            Datetime = date,
                            Buy = buy,
                            Sell = sell,
                            買賣超 = 買賣超
                        };

                        details.Add(dd);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error {e}");
                }
            }
            await context.BulkInsertAsync(details);
        }
    }
}