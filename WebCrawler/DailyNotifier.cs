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
    public class DailyNotifier : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;

        public DailyNotifier(LineNotifyBotApi lineNotifyBotApi)
        { 
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public async Task RunAsync()
        {
            var context = new StockDbContext();
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
            var date = DateTime.Today;

            var tmpPrices = context.Prices.Where(p => 
                p.Datetime == date && 
                p.Close<100 && 
                p.外資買賣超 > 0 && 
                p.投信買賣超 > 0 && 
                (p.主力買超張數 - p.主力賣超張數) > 0
            );

            var prices = from pp in tmpPrices
                         join s in context.Stocks on pp.StockId equals s.StockId
                where s.Industry != "金融保險業"
                select pp;

            var msg = new StringBuilder();
            msg.AppendLine($"外資, 投信, 主力買超股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var price in prices)
            {
                var updown = (price.漲跌 > 0) ? "+" + price.漲跌 : price.漲跌.ToString(CultureInfo.InvariantCulture);
                var updownPercent = (price.漲跌 > 0) ? "+" + price.漲跌百分比 : price.漲跌百分比.ToString(CultureInfo.InvariantCulture);
                msg.AppendLine($"{index}. {price.StockId} {price.Name} {price.Close} {updown}  {updownPercent}%");
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
