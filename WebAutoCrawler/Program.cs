using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using WebAutoCrawler;

namespace WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).ToList();
            var h = new HistoryPriceCrawler();
            var history = new HistoryParser();

            for (int i = 0; i < stocks.Count; i++)
            {
                if (stocks[i].StockId == "2330")
                {

                    var prices = h.Execute(stocks[i].StockId);

                    for (int j  = 0; j < prices.Count; j++)
                    {
                        prices[j].StockId = stocks[i].StockId;
                        stocks[i].Name = stocks[i].Name;
                        history.TrustParser()
  
                    }

                    await context.BulkInsertAsync(prices);
                }
            }


           

            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMinutes);
            Console.ReadLine();
        }
    }
}
