using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class MacdParser : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;
        private string type = "MACD黃金交叉";

        public MacdParser(LineNotifyBotApi lineNotifyBotApi)
        {
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public override async Task RunAsync()
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

                    var existed = await context.RealtimeBestStocks.AnyAsync(p => p.StockId == ss[0] && p.Name == ss[1] && p.Type == type && p.Datetime == DateTime.Today);
                    if (!existed)
                    {
                        context.RealtimeBestStocks.Add(new RealtimeBestStocks
                        {
                            Id = Guid.NewGuid(),
                            StockId = ss[0],
                            Name = ss[1],
                            Datetime = DateTime.Today,
                            Type = type
                        });
                    }
                }
            }
            await context.SaveChangesAsync();
            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
            await NotifyBotApiAsync(context, type);

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
