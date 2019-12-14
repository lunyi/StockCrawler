using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;

namespace WebCrawler
{
    public class RealtimeParser : BaseParser
    {
        public async Task RunAsync()
        {
            var url = $"https://histock.tw/app/table.aspx";
            var rootNode = GetRootNoteByUrl(url);
            var nodes = rootNode.SelectNodes("//*[@id='fm']/div[4]/div[3]/div[1]/div/div/table/tr");

            var s = new Dictionary<string, Stock[]>();

            for (int i = 1; i < nodes.Count; i++)
            {
                var key = nodes[i].SelectSingleNode("th").InnerText;
                var stocks = nodes[i].SelectNodes("td/a");
                if (stocks == null) continue;

                var stockList = new List<Stock>();

                for (int j = 0; j < stocks.Count; j++)
                {
                    var name = stocks[j].InnerText;
                    var stockid = stocks[j].Attributes[0].Value.Substring(7);
                    stockList.Add(new Stock(stockid, name));
                }

                s.Add(key, stockList.ToArray());
            }

            await InsertAsync(s);
        }


        private async Task InsertAsync(Dictionary<string, Stock[]> s)
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1);

            var bestsToRemove = context.RealtimeBestStocks.Where(p => p.Datetime == DateTime.Today);

            foreach (var item in bestsToRemove)
            {
                context.RealtimeBestStocks.Remove(item);
            }

            await context.SaveChangesAsync();

            foreach (var item in s)
            {
                var stockids = item.Value.Select(p=>p.StockId).ToArray();
                var stockList = stocks.Where(s => stockids.Contains(s.StockId)).ToArray();

                foreach (var stock in stockList)
                {
                    var realtime = new RealtimeBestStocks
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.StockId,
                        Name = stock.Name,
                        Type = item.Key,
                        Datetime = DateTime.Today
                    };

                    Console.WriteLine($"{item.Key} {stock.StockId}");
                    context.RealtimeBestStocks.Add(realtime);
                }
                await context.SaveChangesAsync();
            }
        }
    }

    public class Stock 
    {
        public Stock(string stockid, string name)
        {
            StockId = stockid;
            Name = name;
        }
        public string StockId { get; set; }
        public string Name { get; set; }
    }
}
