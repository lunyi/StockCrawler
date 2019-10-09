using DataService.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
                .Where(p => !NotContainStocks.Contains(p.StockId))
                .OrderBy(p => p.StockId)
                .ToArray();

            var parser = new CnyParser();

            foreach (var item in stocks)
            {
                var prices = await Task.Run(() => parser.ParserHistory(item.StockId, item.Name));
                context.Prices.AddRange(prices);
                await context.SaveChangesAsync();
            }

            foreach (var item in parser.ErrorStocks)
            {
                Console.WriteLine($"{item.Key} {item.Value}");
            }

            Console.ReadLine();
        }
    }
}
