using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DataService.Models;

namespace WebCrawler
{
    public class MacdParser : BaseParser
    {
        public async Task RunAsync()
        {
            var context = new StockDbContext();
            var s = Stopwatch.StartNew();
            s.Start();

            var rootNode = GetRootNoteByUrl("https://stock.wearn.com/smart.asp?m1=9&m3=1", false);
            var htmlNode = rootNode.SelectSingleNode("/html/body/div").ChildNodes[17].ChildNodes[1].ChildNodes[5];

            for (int i = 1; i < htmlNode.ChildNodes.Count; i++)
            {
                var tr = htmlNode.ChildNodes[i];

                for (int j = 1; j < tr.ChildNodes.Count; j+=2)
                {
                    var ss = tr.ChildNodes[j].ChildNodes[0].InnerText.Split(' ');
                    context.RealtimeBestStocks.Add(new RealtimeBestStocks
                    { 
                        Id = Guid.NewGuid(),
                        StockId = ss[0],
                        Name = ss[1],
                        Datetime = DateTime.Today,
                        Type = "MACD黃金交叉"
                    });
                }
            }
            await context.SaveChangesAsync();
            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
        }
    }
}
