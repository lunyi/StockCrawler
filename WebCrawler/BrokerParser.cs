using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class BrokerParser : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private readonly string _token;

        public BrokerParser(LineNotifyBotApi lineNotifyBotApi)
        {
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public BrokerParser()
        {
        }
        public async Task GetBrokerInfoAsync()
        {
            var context = new StockDbContext();     
            var reader = new StreamReader(@"D:\Code\BrokerInfo.csv", Encoding.UTF8);
            var data = await reader.ReadToEndAsync();
            var rows = data.Split("\r\n");

            foreach (var row in rows)
            {
                try
                {
                    var col = row.Split(",");
                    var broker = new Broker();

                    var dateString = col[2].Split("/");
                    var year = Convert.ToInt32(dateString[0]) + 1911;
                    var month = Convert.ToInt32(dateString[1]);
                    var day = Convert.ToInt32(dateString[2]);
                    var date = new DateTime(year, month, day);

                    broker = new Broker
                    {
                        //Id = Guid.NewGuid(),
                        //BrokerId = col[0],
                        //BrokerName = col[1],
                        //BusinessDay = date,
                        //Address = col[3],
                        //Tel = col[4]
                    };

                    context.Broker.Add(broker);
                } 
                catch (Exception ex)
                {
                    Console.WriteLine(row);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task RunAsync()
        {
            var startDate = "2020-3-1";
            var endDate = "2020-7-17";
            var s = new Stopwatch();
            s.Start();
            var context = new StockDbContext();
            var stocks = await context.Stocks.Where(p=>p.Status == 1).OrderBy(p => p.StockId).ToArrayAsync();
            var brokers = await context.Broker.OrderBy(p => p.BHID).ToArrayAsync();
            var sb = new StreamWriter(@"D:\Database\StockBroker.csv", true, Encoding.Unicode, 1024);

            sb.AutoFlush = true;

            for (int k = 0; k < stocks.Length; k++)
            {
                var stockId = stocks[k].StockId;
                var stockName = stocks[k].Name;
                s.Restart();
                for (int i = 28; i < brokers.Length; i++)
                {
                    var brokerId = brokers[i].BHID;
                    var sql = $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={brokerId}&b={brokerId}&C=1&D={startDate}&E={endDate}&ver=V3";

                    try
                    {
                        var rootNode = GetRootNoteByUrl(sql, false);
                        var htmlNode = rootNode.SelectSingleNode("//*[@id=\"oMainTable\"]");

                        if (htmlNode == null)
                        {
                            continue;
                        }
                        Console.WriteLine($"{stockId} {stockName} {brokers[i].BHID} {brokers[i].BrokerName}");
                        for (int j = 3; j < htmlNode.ChildNodes.Count; j += 2)
                        {
                            var date = Convert.ToDateTime(htmlNode.ChildNodes[j].ChildNodes[1].InnerHtml);
                            var buy = int.Parse(htmlNode.ChildNodes[j].ChildNodes[3].InnerHtml.Replace(",",""));
                            var sell = int.Parse(htmlNode.ChildNodes[j].ChildNodes[5].InnerHtml.Replace(",", ""));
                            var 買賣超 = int.Parse(htmlNode.ChildNodes[j].ChildNodes[9].InnerHtml.Replace(",", ""));
                            await sb.WriteLineAsync($"{Guid.NewGuid()},{brokers[i].BHID},{brokers[i].BrokerName},{stockId},{stockName},{date:yyyy-MM-dd},{buy},{sell},{買賣超},0");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    Console.WriteLine($"{s.Elapsed.TotalSeconds} sec");
                }
            }
           
            sb.Close();
        }

        public async Task RunAllAsync()
        {
            string path = "";
            var startDate = "2020-3-1";
            var endDate = "2020-7-17";
            var s = new Stopwatch();
            s.Start();
            var context = new StockDbContext();
            var stocks = await context.Stocks.Where(p => p.Status == 1).OrderBy(p => p.StockId).ToArrayAsync();
            var sb = new StreamWriter(@"D:\Database\StockBroker.csv", true, Encoding.Unicode, 1024);

            sb.AutoFlush = true;

            var rangePartitioner = Partitioner.Create(0, stocks.Length);

            Parallel.ForEach(rangePartitioner, (range, loopState) =>
            {
                // Loop over each range element without a delegate invocation.
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Process.Start(path);
                }
            });

            sb.Close();
        }

        public async Task RunByStockIdAsync(string stockId, string startDate, string endDate)
        {
            var s = new Stopwatch();
            s.Start();
            var context = new StockDbContext();
            var stock = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stockId);
            var brokers = await context.Broker.OrderBy(p => p.BrokerName).ToArrayAsync();
            var sb = new StreamWriter(@"D:\Database\StockBroker.csv", true, Encoding.Unicode, 1024);

            sb.AutoFlush = true;

            var stockName = stock.Name;

            s.Restart();
            for (int i = 0; i < brokers.Length; i++)
            {
                var brokerId = brokers[i].BHID;
                var sql = $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={brokerId}&b={brokerId}&C=1&D={startDate}&E={endDate}&ver=V3";

                try
                {
                    var rootNode = GetRootNoteByUrl(sql, false);
                    var htmlNode = rootNode.SelectSingleNode("//*[@id=\"oMainTable\"]");

                    if (htmlNode == null)
                    {
                        continue;
                    }
                    Console.WriteLine($"{stockId} {stockName} {brokers[i].BHID} {brokers[i].BrokerName}");
                    for (int j = 3; j < htmlNode.ChildNodes.Count; j += 2)
                    {
                        var date = Convert.ToDateTime(htmlNode.ChildNodes[j].ChildNodes[1].InnerHtml);
                        var buy = int.Parse(htmlNode.ChildNodes[j].ChildNodes[3].InnerHtml.Replace(",", ""));
                        var sell = int.Parse(htmlNode.ChildNodes[j].ChildNodes[5].InnerHtml.Replace(",", ""));
                        var 買賣超 = int.Parse(htmlNode.ChildNodes[j].ChildNodes[9].InnerHtml.Replace(",", ""));
                        await sb.WriteLineAsync($"{Guid.NewGuid()},{brokers[i].BHID},{brokers[i].BrokerName},{stockId},{stockName},{date:yyyy-MM-dd},{buy},{sell},{買賣超},0");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine($"{s.Elapsed.TotalSeconds} sec");
            }
        }

        public async Task RunByBrokerIdAsync(string brokerId, string startDate, string endDate)
        {
            var s = new Stopwatch();
            s.Start();
            var context = new StockDbContext();
            var broker = await context.Broker.FirstOrDefaultAsync(p => p.BHID == brokerId);
            var stocks = await context.Stocks.OrderBy(p => p.StockId).ToArrayAsync();
            var sb = new StreamWriter(@"D:\Database\Broker.csv", true, Encoding.Unicode, 1024);

            sb.AutoFlush = true;

            var brokerName = broker.BrokerName;

            s.Restart();
            for (int i = 0; i < stocks.Length; i++)
            {
                var stockId = stocks[i].StockId;
                var sql = $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={brokerId}&b={brokerId}&C=1&D={startDate}&E={endDate}&ver=V3";

                try
                {
                    var rootNode = GetRootNoteByUrl(sql, false);
                    var htmlNode = rootNode.SelectSingleNode("//*[@id=\"oMainTable\"]");

                    if (htmlNode == null)
                    {
                        continue;
                    }
                    Console.WriteLine($"{stockId} {stocks[i].Name} {brokerId} {brokerName}");
                    for (int j = 3; j < htmlNode.ChildNodes.Count; j += 2)
                    {
                        var date = Convert.ToDateTime(htmlNode.ChildNodes[j].ChildNodes[1].InnerHtml);
                        var buy = int.Parse(htmlNode.ChildNodes[j].ChildNodes[3].InnerHtml.Replace(",", ""));
                        var sell = int.Parse(htmlNode.ChildNodes[j].ChildNodes[5].InnerHtml.Replace(",", ""));
                        var 買賣超 = int.Parse(htmlNode.ChildNodes[j].ChildNodes[9].InnerHtml.Replace(",", ""));
                        await sb.WriteLineAsync($"{Guid.NewGuid()},{brokerId},{brokerName},{stockId},{stocks[i].Name},{date:yyyy-MM-dd},{buy},{sell},{買賣超},0");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine($"{s.Elapsed.TotalSeconds} sec");
            }
        }

        private async Task NotifyBotApiAsync(StockDbContext context, string type)
        {
            var stocks = context.RealtimeBestStocks
                .Where(p => p.Datetime == DateTime.Today && p.Type == type)
                .OrderBy(p => p.StockId).ToArray();

            if (stocks.Any())
            {
                var s = new StringBuilder();
                s.AppendLine($@"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {type}");

                foreach (var stock in stocks)
                {
                    s.AppendLine($@"{stock.StockId} {stock.Name}");
                }

                await NotifyBotApiAsync(s.ToString());
            }
        }

        private async Task NotifyBotApiAsync(string message)
        {
            await _lineNotifyBotApi.Notify(new NotifyRequestDTO
            {
                AccessToken = _token,
                Message = message
            });
        }
    }
}
