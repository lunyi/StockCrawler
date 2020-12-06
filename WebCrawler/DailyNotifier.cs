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

        public override async Task RunAsync()
        {
            var context = new StockDbContext();
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
            var 外資投信主力買超股票 = Get外資投信主力買超股票(context);
            var 董監買超股票 = await GetSqlByChairmanAsync(context);
            var 投信突然加入買方 = await Get投信突然加入買方Sql(context);
            var 均線起飛第一天Sql = 均線起飛第一天(context);
            await NotifyBotApiAsync(外資投信主力買超股票);
            await NotifyBotApiAsync(董監買超股票);
            await NotifyBotApiAsync(投信突然加入買方);
            await NotifyBotApiAsync(均線起飛第一天Sql);
        }

        private string Get外資投信主力買超股票(StockDbContext context)
        {
            var tmpPrices = context.Prices.Where(p =>
               p.Datetime == DateTime.Today &&
               p.Close < 150 &&
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

        private string 均線起飛第一天(StockDbContext context)
        {
            var prices = context.Prices.Where(p =>
               p.Datetime == DateTime.Today &&
               p.漲跌百分比 > 0 && p.AvgUpDays ==1
           ) ;

            var msg = new StringBuilder();
            msg.AppendLine($"均線起飛第一天股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

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
select s.[StockId]
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
where a.[董監持股] -  b.[董監持股] > 100) b on s.StockId = b.StockId
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



        private async Task<string> Get投信突然加入買方Sql(StockDbContext context)
        {
            var datetime = DateTime.Today.ToString("yyyy-MM-dd");
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(5)
                .OrderBy(p => p.Datetime)
                .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
                .FirstOrDefault();

            var sql = $@"
  WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and [Datetime] >= '{datetime2}'
)

select 
      s.[StockId]
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
	  ,CAST(t1.[投信買賣超] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join TOPTEN t1 on s.StockId = t1.StockId
join TOPTEN t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
join TOPTEN t4 on t1.StockId = t4.StockId and t1.RowNo + 3 = t4.RowNo
join TOPTEN t5 on t1.StockId = t5.StockId and t1.RowNo + 4 = t5.RowNo
join TOPTEN t6 on t1.StockId = t6.StockId and t1.RowNo + 5 = t6.RowNo
WHERE t1.RowNo=1 and 
t1.[投信買賣超] > 0 and 
t2.[投信買賣超] <= 0 and
t3.[投信買賣超] <= 0 and
t4.[投信買賣超] <= 0 and
t5.[投信買賣超] <= 0 and
t6.[投信買賣超] <= 0
order by t1.[投信買賣超] desc
";

            var stocks = await context.Stocks.FromSqlRaw(sql).ToArrayAsync();
            var msg = new StringBuilder();
            msg.AppendLine($"投信突然加入買方 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

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
