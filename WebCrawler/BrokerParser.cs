using System;
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
        private string _token;

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
                        Id = Guid.NewGuid(),
                        BrokerId = col[0],
                        BrokerName = col[1],
                        BusinessDay = date,
                        Address = col[3],
                        Tel = col[4]
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

        public override async Task RunAsync()
        {
            var startDate = "2019-9-1";
            var endDate = "2020-5-8";
            var broker = "5850";
            var stockId = "2455";

            var context = new StockDbContext();
            var brokers = await context.Broker.ToArrayAsync();

            for (int i = 0; i < brokers.Length; i++)
            {
                var sql = $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={brokers[i].BrokerId}&b={broker}&C=1&D={startDate}&E={endDate}&ver=V3";
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
