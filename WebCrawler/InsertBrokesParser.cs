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
        public async Task RunAsync(string stockId, string name, int index, int partition)
        {
            stockId = "1101";
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new StockDbContext();

            var brokers = context.Broker.ToList();
            int start = (index - 1) * brokers.Count / partition;
            int end = index * brokers.Count / partition;

            var startDate = "2019-9-1";
            var endDate = "2021-1-4";
            var details = new List<BrokerTransactionDetails>();
            var seq = 0;

            var ss = Stopwatch.StartNew();
            ss.Start();
            for (int i = start; i < end; i++)
            {
                seq++;
                Console.WriteLine($"{seq}/{end - start} {stockId} {brokers[i].BrokerName} {brokers[i].BHID} {brokers[i].BrokerName}");
             
                try
                {
                    //https://djinfo.cathaysec.com.tw/z/zc/zco/zco0/zco0.djhtm?A=1783&BHID=9600&b=003900360035004b&C=1&D=2020-1-1&E=2020-12-23&ver=V3
                    //https://moneydj.emega.com.tw/z/zc/zco/zco0/zco0.djhtm?A=6486&BHID=9600&b=003900360035004b&C=1&D=2020-1-1&E=2021-1-6&ver=V3
                    //https://tv.moneydj.com/z/zc/zco/zco0/zco0.djhtm?a=5289&BHID=9600&b=003900360035004b&d=2020-1-1&e=2021-1-6
                    //https://concords.moneydj.com/z/zc/zco/zco0/zco0.djhtm?A=6104&BHID=5850&b=0035003800350063&C=1&D=2020-1-1&E=2021-1-6&ver=V3
                    //http://just2.entrust.com.tw/z/zc/zco/zco0/zco0.djhtm?A=3189&BHID=9600&b=0039003600390043&C=1&D=2020-1-1&E=2021-1-6&ver=V3
                    //http://stockchannel.sinotrade.com.tw/z/zc/zco/zco0/zco0.djhtm?A=6104&BHID=5850&b=0035003800350063&C=1&D=2020-1-1&E=2021-1-6&ver=V3
                    //http://jsjustweb.jihsun.com.tw/z/zc/zco/zco0/zco0.djhtm?A=4960&BHID=1160&b=0031003100360045&C=1&D=2020-1-1&E=2021-1-6&ver=V3            
                    //http://5850web.moneydj.com/z/zc/zco/zco0/zco0.djhtm?A=4960&BHID=1160&b=0031003100360045&C=1&D=2020-1-1&E=2021-1-6&ver=V3
                    //http://sod.nsc.com.tw/z/zc/zco/zco0/zco0.djhtm?A=6213&BHID=1160&b=0031003100360045&C=1&D=2020-1-1&E=2021-1-6&ver=V3
                    //http://jdata.yuanta.com.tw/z/zc/zco/zco0/zco0.djhtm?A=3207&BHID=1160&b=0031003100360045&C=1&D=2016-1-1&E=2021-1-11&ver=V3
                    var url = $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={brokers[i].BHID}&b={brokers[i].b}&C=1&D={startDate}&E={endDate}&ver=V3";
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
                            BrokerId = brokers[i].b,
                            BrokerName = brokers[i].BrokerName,
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
            ss.Stop();
            Console.WriteLine($"Elapsed: {ss.Elapsed.TotalSeconds}");
        }
    }
}