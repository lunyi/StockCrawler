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
            var h = new HistoryPriceCrawler();
            var prices = h.Execute("2330");

            for (int i = 0; i < prices.Count; i++)
            {
                prices[i].StockId
            }

             var history = new HistoryParser();
            history.TrustParser()
            await context.BulkInsertAsync(prices);
            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMinutes);
            Console.ReadLine();
        }
    }
}
