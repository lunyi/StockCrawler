using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using WebAutoCrawler;

namespace WebCrawler
{
    class Program
    {
        [Obsolete]
        static async Task Main(string[] args)
        {
            var c = new ThousandDataCrawlerV2();
            await c.ExecuteLastAsync();
        }

        private async Task ParseHistory()
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p=>p.StockId).ToList();
            var h = new HistoryPriceCrawler();
            var history = new HistoryParser();

            for (int i = 0; i < stocks.Count; i++)
            {
                if (stocks[i].StockId == "2330")
                {
                    var prices = h.Execute(stocks[i].StockId);

                    for (int j = 0; j < prices.Count; j++)
                    {
                        prices[j].StockId = stocks[i].StockId;
                        prices[j].Name = stocks[i].Name;
                        history.ParseTrust(prices, prices[j].StockId, prices[j].Datetime.ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        history.ParseFinancing(prices, prices[j].StockId, prices[j].Datetime.ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        decimal oneDayBuy = 0, OneDaySell = 0;
                        Thread.Sleep(1000);
                        (oneDayBuy, OneDaySell) = history.ParseMainForce(prices[j].StockId, prices[j].Datetime.ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));

                        prices[j].主力買超張數 = oneDayBuy;
                        prices[j].主力賣超張數 = OneDaySell;

                        (oneDayBuy, OneDaySell) = history.ParseMainForce(prices[j].StockId, prices[j].Datetime.AddDays(-6).ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        prices[j].五日主力買超張數 = oneDayBuy;
                        prices[j].五日主力賣超張數 = OneDaySell;

                        (oneDayBuy, OneDaySell) = history.ParseMainForce(prices[j].StockId, prices[j].Datetime.AddDays(-13).ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        prices[j].十日主力買超張數 = oneDayBuy;
                        prices[j].十日主力賣超張數 = OneDaySell;

                        (oneDayBuy, OneDaySell) = history.ParseMainForce(prices[j].StockId, prices[j].Datetime.AddDays(-27).ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        prices[j].二十日主力買超張數 = oneDayBuy;
                        prices[j].二十日主力賣超張數 = OneDaySell;

                        (oneDayBuy, OneDaySell) = history.ParseMainForce(prices[j].StockId, prices[j].Datetime.AddDays(-55).ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        prices[j].四十日主力賣超張數 = oneDayBuy;
                        prices[j].四十日主力賣超張數 = OneDaySell;

                        (oneDayBuy, OneDaySell) = history.ParseMainForce(prices[j].StockId, prices[j].Datetime.AddDays(-83).ToString("yyyy/MM/dd"), prices[j].Datetime.ToString("yyyy/MM/dd"));
                        prices[j].六十日主力買超張數 = oneDayBuy;
                        prices[j].六十日主力賣超張數 = OneDaySell;
                    }

                    await context.BulkInsertAsync(prices);
                }
            }

            s.Stop();
            Console.WriteLine(s.Elapsed.TotalSeconds);
            Console.ReadLine();
        }
    }
}
