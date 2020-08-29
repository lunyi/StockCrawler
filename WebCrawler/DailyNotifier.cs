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
            var 外資投信主力買超股票 = Get外資投信主力買超股票(context);
            var 董監買超股票 = await GetSqlByChairmanAsync(context);
            await NotifyBotApiAsync(外資投信主力買超股票);
            await NotifyBotApiAsync(董監買超股票);
        }

        private string Get外資投信主力買超股票(StockDbContext context)
        {
            var tmpPrices = context.Prices.Where(p =>
               p.Datetime == DateTime.Today &&
               p.Close < 100 &&
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

            return msg.ToString();
        }

        private async Task<string> GetSqlByChairmanAsync(StockDbContext context)
        {
            var datetime = DateTime.Today.ToString("yyyy-MM-dd");
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(1)
                .Select(p => p.Datetime)
                .FirstOrDefault().ToString("yyyy/MM/dd");

            var sql = $@"
select s.[Id]
      ,s.[StockId]
      ,s.[Name]
      ,s.[MarketCategory]
      ,s.[Industry]
      ,s.[ListingOn]
      ,s.[CreatedOn]
      ,s.[UpdatedOn]
      ,s.[Status]
      ,s.[Address]
      ,s.[Website]
      ,s.[營收比重]
      ,s.[股本]
      ,s.[股價]
      ,s.[每股淨值]
      ,s.[每股盈餘], s.[ROE], s.[ROA]
	  ,CAST(b.買超 AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks] s join (
select a.StockId, a.NAme, a.[董監持股]- b.[董監持股] as 買超
from (
select * from [Prices] 
where [Datetime] = '{datetime}' ) a 
join (
select * from [Prices] 
where [Datetime] = '{datetime2}' ) b  on a.StockId = b.StockId
where a.[董監持股] !=  b.[董監持股]) b on s.StockId = b.StockId
order by b.買超 desc
";

            var stocks = await context.Stocks.FromSqlRaw(sql).ToArrayAsync();

            var msg = new StringBuilder();
            msg.AppendLine($"董監買超股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.StockId} {stock.Name} {stock.股價} {stock.Description}");
                index++;
            }
            return msg.ToString();
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
