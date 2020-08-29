using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class DailyKLineNotifier : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;

        public DailyKLineNotifier(LineNotifyBotApi lineNotifyBotApi)
        { 
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public async Task RunAsync(int minutes)
        {
            var context = new StockDbContext();
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();

            context.Database.ExecuteSqlCommand("exec [usp_GetMinuteKLine] @p0, @p1", minutes, DateTime.Today.ToString("yyyy-MM-dd"));
            var key = $"_{minutes}分K線均線多排";
            var stocks = await context.BestStocks.Where(p => p.Type == key)
                .OrderBy(p=>p.StockId)
                .ToListAsync();

            var msg = new StringBuilder();
            msg.AppendLine($"{key} : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.StockId}{stock.Name}  {stock.CreatedOn:HH:mm}");
                index++;
            }

            await NotifyBotApiAsync(msg.ToString());
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
