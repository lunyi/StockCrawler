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
    public class WeekNotifier : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;

        public WeekNotifier(LineNotifyBotApi lineNotifyBotApi)
        { 
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public override async Task RunAsync()
        {
            var context = new StockDbContext();
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
            //var 外資投信主力買超股票 = Get外資投信主力買超股票(context);

            var 五日漲幅排行榜 = 五日漲幅排行榜1(context);
            var 主力外資融資大買 = 主力外資融資大買1(context);
            var 大戶比例增加 = 大戶比例增加1(context);

 
            await NotifyBotApiAsync(五日漲幅排行榜);
            await NotifyBotApiAsync(主力外資融資大買);
            await NotifyBotApiAsync(大戶比例增加);
        }

        private string 主力外資融資大買1(StockDbContext context)
        {
            context = new StockDbContext();
            var datetime2 = context.Thousand.Where(p => p.StockId == "2330")
    .OrderByDescending(p => p.Datetime)
    .Take(1)
    .OrderBy(p => p.Datetime)
    .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
    .FirstOrDefault();

            var stocks = context.Stocks.FromSqlRaw($@"exec [usp_GetWeekBestStocks] '{datetime2}'");
            var msg = new StringBuilder();
            msg.AppendLine($"主力外資融資大買 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.StockId} {stock.Name} {stock.股價} => {stock.Description.Replace(".000","")}");
                index++;
            }
            return msg.ToString();
        }

        private string 五日漲幅排行榜1(StockDbContext context)
        {
            context = new StockDbContext();
            var sql = $@"
declare @date as Datetime = '{DateTime.Today:yyyy-MM-dd}';

WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name]
       order by [CreatedOn] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime]>= DATEADD(DAY, -7 , @date) and [Datetime] <= @date
)

SELECT top 100 StockId, Name, sum(漲跌百分比) as [Description]
into #tmp
FROM TOPTEN 
WHERE RowNo <= 7
Group by StockId, Name
order by sum(漲跌百分比) desc

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
      ,CAST(t.[Description] AS nvarchar(30)) AS [Description]
      ,股票期貨
from Stocks s 
join #tmp t on t.StockId = s.StockId
order by t.[Description] desc
drop table #tmp
";
            var stocks = context.Stocks.FromSqlRaw(sql);

            var msg = new StringBuilder();
            msg.AppendLine($"五日漲幅排行榜 : {DateTime.Now:yyyy-MM-dd}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.StockId} {stock.Name} {stock.股價} {stock.Description}%");
                index++;
            }
            return msg.ToString();
        }
        private string 大戶比例增加1(StockDbContext context)
        {
            context = new StockDbContext();
            var sql =
$@"
WITH TOPTEN as (
   SELECT*, ROW_NUMBER()
    over(
        PARTITION BY[Name]
       order by[Datetime] desc
    ) AS RowNo
    FROM[Thousand] where[Datetime] >= DATEADD(DD, -7, '{DateTime.Today:yyyy-MM-dd}') and[Datetime] <= '{DateTime.Today:yyyy-MM-dd}'
)

select *, (POver1000 - PPOver1000) as [Percent]
into #tmp
from[TOPTEN]
where RowNo = 1 and(POver1000 - PPOver1000) > 1 and(PUnder100 < PPUnder100)

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
	  ,CAST(t.POver1000 - t.PPOver1000 AS nvarchar(30)) AS [Description]
      ,s.股票期貨
from[Stocks]s
join #tmp t on s.StockId = t.StockId
order by(t.POver1000 - t.PPOver1000) desc

drop table #tmp
";
            var stocks = context.Stocks.FromSqlRaw(sql).ToArray();

            var msg = new StringBuilder();
            msg.AppendLine($"大戶比例增加 : {DateTime.Now:yyyy-MM-dd}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.StockId} {stock.Name} {stock.股價} => {stock.Description}%");
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
