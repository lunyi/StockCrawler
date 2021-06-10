using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PostgresData.Models;
using EFCore.BulkExtensions;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class WangooParser : BaseParser
    {
        private string _token;

        public ConcurrentDictionary<string, string> ErrorStock { get; set; }

        [Obsolete]
        public async Task RunAsync(int index, int partition)
        {

            ExecuteByStock2("1101", "");
            //ExecuteByStock("2642", "2642");
            var context = new stockContext();
            var s = Stopwatch.StartNew();
            s.Start();

            var stocks = await context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToArrayAsync();

            int start = (index - 1) * stocks.Length / partition;
            int end = index * stocks.Length / partition;

            var Price = new List<Price>();
            var seq = 0;
            for (int i = start; i < end; i++)
            {
                try
                {
                    //var oldPrice = oldPrice.FirstOrDefault(p => p.StockId == stocks[i].StockId);
                    //var price = ExecuteByStock(oldPrice, stocks[i].StockId, stocks[i].Name);
                    //Price.Add(price);

                    Console.WriteLine($"{index}/{partition}  {seq++}/{end - start}  {stocks[i].StockId} {stocks[i].Name}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{ stocks[i].StockId} { stocks[i].Name} : Error {e}");
                }
            }

            context.Database.SetCommandTimeout(180);

            Price = Price.Where(p => p != null).ToList();
            try 
            {
                await context.BulkInsertOrUpdateAsync(Price);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{index}/{partition}");
                Console.WriteLine($"Error {e}");

            }
            context.Database.SetCommandTimeout(300);
            context.Database.ExecuteSqlRaw($"exec [usp_Update_MA_And_VMA] {DateTime.Today:yyyy-MM-dd}");
            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
        }

        private Price ExecuteByStock(string stockid, string name)
        {
            var url = $"https://histock.tw/stock/{stockid}";
            var rootNode = GetRootNoteByUrl(url);
            var htmlNode4 = rootNode.SelectSingleNode("//*[@id=\"fm\"]/div[4]/div[3]/div/div[1]/div[1]/div[2]/div[1]/div[2]/ul");

            var price = new Price
            {


            };
            return price;
        }

        private Price ExecuteByStock2(string stockid, string name)
        {
            var url = $"https://www.cmoney.tw/finance/f00025.aspx?s={stockid}";
            var rootNode = GetRootNoteByUrl(url);
            var htmlNode1 = rootNode.SelectSingleNode("//*[@id=\"HeaderContent\"]");
            var htmlNode2 = rootNode.SelectSingleNode("//*[@id=\"HeaderContent\"]/ul");
            var htmlNode3 = rootNode.SelectSingleNode("//*[@id=\"HeaderContent\"]/ul/li[1]");
            var htmlNode4 = rootNode.SelectSingleNode("//*[@id=\"HeaderContent\"]/ul/li[1]/div[2]");

            var price = new Price
            {


            };
            return price;
        }
    }
}
