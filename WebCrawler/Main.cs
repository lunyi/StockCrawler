using DataService.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace ConsoleApp
{
    class CheckModel
    {
        static string[] NotContainStocks = new string[] {
                "0050",
                "0051",
                "0052",
                "0053",
                "0054",
                "0055",
                "0056",
                "0057",
                "0058",
                "0059",
                "0060",
                "0061"
            };

        static async Task Main(string[] args)
        {
            var context = new StockDbContext();


            var stocks = context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToList();

            //var stocks = context.Stocks
            //    .Where(p => !NotContainStocks.Contains(p.StockId))
            //    .OrderBy(p => p.StockId)
            //    .ToArray();

            var s = Stopwatch.StartNew();
            s.Start();

            var parser = new CnyParser();

            foreach (var item in stocks)
            {
                await ExecuteLastAsync(parser, context, item.StockId, item.Name);
            }

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");

            var sql = GetSql();
            stocks = context.Stocks.FromSqlRaw(sql).ToList();

            foreach (var item in stocks)
            {
                Console.WriteLine($"Error: {item.StockId} {item.Name}");
                await ExecuteLastAsync(parser, context, item.StockId, item.Name);

            }

            Console.ReadLine();
        }

        private static async Task ExecuteHistoryAsync(CnyParser parser, StockDbContext context, string stockId, string name)
        {
            var prices = parser.ParserHistory(stockId, name);

            foreach (var price in prices)
            {
                var p = context.Prices.FirstOrDefault(p => p.Datetime == price.Datetime && p.StockId == stockId);
                if (p == null)
                {
                    context.Prices.Add(price);
                }        
            }
            Console.WriteLine($"Finished: {stockId} {name}");
          
            await context.SaveChangesAsync();
        }

        private static async Task ExecuteLastAsync(CnyParser parser, StockDbContext context, string stockId, string name)
        {
            var price = parser.ParserLastDay(stockId, name);

            if (price == null)
            {
                return;
            }
            var p = context.Prices.FirstOrDefault(p => p.Datetime == price.Datetime && p.StockId == stockId);
            if (p == null)
            {
                context.Prices.Add(price);
            }

            await context.SaveChangesAsync();
        }

        private static string GetSql()
        {
            return @$"
select * from [Stocks]
where StockId not in (
SELECT StockId
  FROM [dbo].[Prices]
  where [Datetime] = '{DateTime.Today.ToString("yyyy/MM/dd")}')
 and [Status] = 1
  order by StockId";
        }
    }
}
