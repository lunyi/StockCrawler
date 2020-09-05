using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.DataModel;
using DataService.Enums;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface IStockQueries
    {
        Task<StockeModel> GetPricesByStockIdAsync(string stockId, DateTime datetime, bool chkDate);
        Task<Stocks[]> GetBestStocksAsync(int key);
        Task<string[]> GetDaysAsync();
        Task<string[]> GetChosenStockTypesAsync();
        Task<Stocks[]> GetStocksByDateAsync(string datetime, int type);
        Task SetBestStockAsync(string stockId, string type);
        Task<Stocks[]> GetActiveStocksAsync();
        Task<Stocks[]> GetStocksBySqlAsync(string sql);
        Task<Stocks[]> GetStocksByTypeAsync(string type);
        Task<BestStockType[]> GetBestStockTypeAsync();
        Task<Stocks[]> GetStocksByBestStockTypeAsync(string name, string datetime);
        Task<TwStock[]> GetTwStocksAsync();
        Task<string> GetTokenAsync();
        Task<string[]> GetMinuteKLinesAsync();
    }
    public class StockQueries : IStockQueries
    {
        private StockDbContext _context;
        public StockQueries()
        {
            _context = new StockDbContext();
        }

        public StockQueries(StockDbContext context)
        {
            _context = context;
        }

        async Task<Stocks[]> IStockQueries.GetStocksByDateAsync(string datetime, int type)
        {
            var context = new StockDbContext();
            string sql = string.Empty;

            switch (type)
            {
                case (int)ChooseStockType.五日漲幅排行榜:
                    sql = await 五日漲幅排行榜(context, datetime, 5);
                    break;
                case (int)ChooseStockType.二十日漲幅排行:
                    sql = await 五日漲幅排行榜(context, datetime, 20);
                    break;
                case (int)ChooseStockType.四十日漲幅排行:
                    sql = await 五日漲幅排行榜(context, datetime, 40);
                    break;
                case (int)ChooseStockType.六十日漲幅排行:
                    sql = await 五日漲幅排行榜(context, datetime, 60);
                    break;
                case (int)ChooseStockType.五日跌幅排行榜:
                    sql = 五日跌幅排行榜(datetime, 5);
                    break;
                case (int)ChooseStockType.投信連續買超排行榜:
                    sql = 連續買超排行榜(datetime, "投信買賣超");
                    break;
                case (int)ChooseStockType.投信連續賣超排行榜:
                    sql = 連續賣超排行榜(datetime, "投信買賣超");
                    break;
                case (int)ChooseStockType.外資連續買超排行榜:
                    sql = 連續買超排行榜(datetime, "外資買賣超");
                    break;
                case (int)ChooseStockType.外資連續賣超排行榜:
                    sql = 連續賣超排行榜(datetime, "外資買賣超");
                    break;
                case (int)ChooseStockType.MACD和KD同時轉上:
                    sql = MACD和KD同時轉上(datetime);
                    break;
                case (int)ChooseStockType.融資連續買超排行榜:
                    sql = 融資連續買超排行榜(datetime, true);
                    break;
                case (int)ChooseStockType.融資連續賣超排行榜:
                    sql = 融資連續買超排行榜(datetime, false);
                    break;
                case (int)ChooseStockType.ROE大於15且股價小於50:
                    sql = ROE大於15且股價小於50(datetime);
                    break;
                case (int)ChooseStockType.主力連續買超排行榜:
                    sql = 真主力連續買超排行榜(datetime);
                    break;
                case (int)ChooseStockType.主力連續賣超排行榜:
                    sql = 真主力連續買超排行榜(datetime, false);
                    break;
                case (int)ChooseStockType.買方籌碼集中排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 1, "desc");
                    break;
                case (int)ChooseStockType.五日買方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 5, "desc");
                    break;
                case (int)ChooseStockType.十日買方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 10, "desc");
                    break;
                case (int)ChooseStockType.二十日買方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 20, "desc");
                    break;
                case (int)ChooseStockType.六十日買方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 60, "desc");
                    break;
                case (int)ChooseStockType.連續十二月單月年增率成長:
                    sql = 連續十二月單月年增率成長(datetime);
                    break;
                case (int)ChooseStockType.近月營收累積年增率成長:
                    sql = 近月營收累積年增率成長(datetime);
                    break;
                case (int)ChooseStockType.當週大戶比例增加:
                    sql = Get當週大戶比例增加(datetime);
                    break;
                case (int)ChooseStockType.Get淨值比小於2AndROE大於10:
                    sql = Get淨值比小於2AndROE大於10();
                    break;
                case (int)ChooseStockType.投信突然進前20名:
                    sql = 突然進前20名(datetime, "投信買賣超");
                    break;
                case (int)ChooseStockType.投量比加主力買超:
                    sql = 投量比加主力買超(datetime);
                    break;
                case (int)ChooseStockType.每周投信買散戶賣:
                    sql = 每周投信買散戶賣(context, datetime);
                    break;
                case (int)ChooseStockType.連續上漲天數:
                    sql = 連續上漲天數(datetime);
                    break;
                case (int)ChooseStockType.投信突然加入買方:
                    sql = Get投信突然加入買方Sql(datetime);
                    break;
                case (int)ChooseStockType.賣方籌碼集中排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 1, "asc");
                    break;
                case (int)ChooseStockType.五日賣方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 5, "asc");
                    break;
                case (int)ChooseStockType.十日賣方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 10, "asc");
                    break;
                case (int)ChooseStockType.二十日賣方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 20, "asc");
                    break;
                case (int)ChooseStockType.六十日賣方籌碼集中度排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, 60, "asc");
                    break;
                case (int)ChooseStockType.半年線附近:
                    sql = Get半年線附近Sql(datetime);
                    break;
                case (int)ChooseStockType.連續大戶增加:
                    sql = Get大戶增加散戶減少Sql(datetime);
                    break;
                case (int)ChooseStockType.連續兩週大戶增散戶減:
                    sql = 連續兩週大戶增散戶減(datetime);
                    break;
                case (int)ChooseStockType.董監買賣超排行榜:
                    sql = GetSqlByChairmanAsync(datetime);
                    break;
                case (int)ChooseStockType.每周成交量增長排行榜:
                    sql = 每周成交量增長排行榜(datetime);
                    break;
                case (int)ChooseStockType.多頭排列:
                    sql = Get多頭排列SQL(datetime);
                    break;
                case (int)ChooseStockType.上漲破五日均:
                    sql = Get上漲破五日均SQL(datetime);
                    break;
                case (int)ChooseStockType.上漲破月線:
                    sql = Get上漲破月線SQL(datetime);
                    break;
                case (int)ChooseStockType.三天漲百分之二十:
                    sql = Get三天漲百分之二十SQL(datetime);
                    break;
                case (int)ChooseStockType.三天漲百分之二十且投信買超:
                    sql = Get三天漲百分之二十且投信買超SQL(datetime);
                    break;
                case (int)ChooseStockType.連續兩天漲停板:
                    sql = 連續兩天漲停板(context, datetime);
                    break;
                default:
                    var whereCondition = DateFunc[(ChooseStockType)type]();
                    sql = @$"SELECT s.*
                          FROM [dbo].[Prices] p
                          join Stocks s on p.StockId = s.StockId
                          where [Datetime] = '{datetime}' {whereCondition}";
                    break;
            }

            var res = await context.Stocks.FromSqlRaw(sql).ToArrayAsync();
            return res;
        }

        private string MACD和KD同時轉上(string datetime)
        {
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
                .FirstOrDefault();

            return @$"
  select s.* from Stocks s join (SELECT *
  FROM [dbo].[Prices]
  where [Datetime] ='{datetime}' and [漲跌百分比] > 2 and MA20_ like '%↗' and K1 like '%↗' and OSC1 like '%↗' and [Close] > MA20) a1 on s.StockId = a1.StockId
  join  (SELECT *
  FROM [dbo].[Prices]
  where [Datetime] = '{datetime2}' and  K1 like '%↘' and OSC1 like '%↘') a2  on a1.StockId = a2.StockId
  order by s.StockId";
        }

        private string GetMainForceSql(string mainForce)
        {
            return @$"
  DECLARE @ColumnGroup NVARCHAR(MAX), @PivotSQL NVARCHAR(MAX) 

  SELECT @ColumnGroup = COALESCE(@ColumnGroup + ',' ,'' ) + QUOTENAME([NAme])
  FROM dbo.[Stocks] where [Status] = 1

  SELECT @PivotSQL = N'
  select * from 
  (select  [Datetime], [Name], [外資買賣超]
  from [Prices]) t 
  pivot  (
	MAX([外資買賣超]) 
	for	[Name] in (' +@ColumnGroup+ ')
  ) p order by [Datetime] desc'

  EXEC sp_executesql  @PivotSQL;";
        }

        private string Get籌碼集中排行榜Sql(string datetime, int days, string orderby = "desc")
        {
            string strDays = string.Empty;
            string strVolumn = "成交量";
            switch (days)
            {
                case 5: 
                    strDays = "五日";
                    strVolumn = "VMA5 * 5";
                    break;
                case 10: 
                    strDays = "十日";
                    strVolumn = "VMA10 * 10";
                    break;
                case 20: 
                    strDays = "二十日";
                    strVolumn = "VMA20 * 20";
                    break;
                case 60: 
                    strDays = "六十日";
                    strVolumn = "VMA60 * 60";
                    break;
                default: break;
            }
            return $@"
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
	  ,s.[Description]
      ,s.[股票期貨]
from [Prices] p join [Stocks] s on s.StockId = p.StockId 
where p.[Datetime] = '{datetime}' and [漲跌百分比] > 2 and VMA5 > 0 and VMA10 > 0 and VMA20 > 0 and VMA60 > 0 and p.{strVolumn} > 0
order by (p.[{strDays}主力買超張數] - p.[{strDays}主力賣超張數]) / p.{strVolumn} {orderby}
";
        }

        async Task<StockeModel> IStockQueries.GetPricesByStockIdAsync(string stockId, DateTime datetime, bool chkDate)
        {
            var context = new StockDbContext();
            var prices = await (from price in context.Prices
                                join stock in context.Stocks on price.StockId equals stock.StockId
                                where price.StockId == stockId && (chkDate ? price.Datetime <= datetime : true)
                                orderby price.Datetime descending
                                let volume = price.成交量 == 0 ? 1 : price.成交量
                                select new PriceModel
                                {
                                    StockId = price.StockId,
                                    Name = price.Name,
                                    Datetime = price.Datetime.ToString("yyyy-MM-dd"),
                                    Open = price.Open,
                                    High = price.High,
                                    Low = price.Low,
                                    Close = price.Close,
                                    漲跌 = price.漲跌,
                                    漲跌百分比 = price.漲跌百分比,
                                    成交量 = price.成交量,
                                    本益比 = price.本益比,
                                    //股價淨值比 = Math.Round(price.Close / stock.每股淨值.Value, 2),
                                    外資持股比例 = price.外資持股比例,
                                    投信持股比例 = price.投信持股比例,
                                    董監持股比例 = price.董監持股比例,
                                    融資買賣超 = price.融資買進 - price.融資賣出,
                                    融券買賣超 = price.融券買進 - price.融券賣出,
                                    融券餘額 = price.融券餘額,
                                    董監持股 = price.董監持股,
                                    融資使用率 = price.融資使用率,
                                    外資買賣超 = price.外資買賣超,
                                    自營商買賣超 = price.自營商買賣超,
                                    投信買賣超 = price.投信買賣超,
                                    主力買賣超 = price.主力買超張數 - price.主力賣超張數,
                                    籌碼集中度 = 100 * Math.Round(((price.主力買超張數 - price.主力賣超張數) / volume).Value, 4),
                                    MA5 = price.MA5_ ?? string.Empty,
                                    MA10 = price.MA10_ ?? string.Empty,
                                    MA20 = price.MA20_ ?? string.Empty,
                                    MA60 = price.MA60_ ?? string.Empty,
                                    當沖張數 = price.當沖張數,
                                    當沖比例 = price.當沖比例,
                                    K9 = price.K1,
                                    D9 = price.D1,
                                    MACD = price.MACD1,
                                    OSC = price.OSC1,
                                    DIF = price.DIF1,
                                    //五日籌碼集中度 = 100 * Math.Round(((price.五日主力買超張數 - price.五日主力賣超張數) / (5 * price.VMA5)).Value, 4),
                                    十日籌碼集中度 = 100 * Math.Round(((price.十日主力買超張數 - price.十日主力賣超張數) / (10 * price.VMA10)).Value, 4),
                                    //二十日籌碼集中度 = 100 * Math.Round(((price.二十日主力買超張數 - price.二十日主力賣超張數) / (20 * price.VMA20)).Value, 4),
                                    //六十日籌碼集中度 = 100 * Math.Round(((price.六十日主力買超張數 - price.六十日主力賣超張數) / (60 * price.VMA60)).Value, 4),
                                    //周轉率 = 100 * Math.Round(((decimal)price.成交量 / price.發行張數).Value, 5)
                                }).ToArrayAsync();

            var datetimeString = datetime.ToString("yyyy-MM-dd");
            var weeklyChip = await context._WeekyChip.FromSqlRaw(GetWeekAnalyst(stockId, GetLastFriday(datetimeString), chkDate)).ToArrayAsync();
            var monthData = await context._MonthData.FromSqlRaw("exec [usp_GetMonthData] {0}, {1}", stockId, datetimeString).ToArrayAsync();

            return new StockeModel
            {
                Stock = await context.Stocks.FirstOrDefaultAsync(p=>p.StockId == stockId),
                Prices = prices,
                WeeklyChip = weeklyChip,
                MonthData = monthData
            };
        }
        Task<TwStock[]> IStockQueries.GetTwStocksAsync()
        {
            var context = new StockDbContext();
            return context.TwStock.OrderByDescending(p=>p.Datetime).Take(220).ToArrayAsync();
        }

        private string GetLastFriday(string datetime = null)
        {
            if (datetime == null)
            {
                int days1 = DateTime.Today.DayOfWeek == DayOfWeek.Saturday ? 1 : 1 * ((int)DateTime.Today.DayOfWeek + 2);
                return DateTime.Today.AddDays(days1 * -1).ToString("yyyy-MM-dd");
            }

            var today = Convert.ToDateTime(datetime);
            var days = today.DayOfWeek == DayOfWeek.Saturday ? 1 : 1 * ((int)today.DayOfWeek + 2);
            days = days == 7 ? 0 : days;
            return today.AddDays(days * -1).ToString("yyyy-MM-dd");
        }

        private string GetLastMonday(DateTime lastThousandDay)
        {
            var days = (int)(lastThousandDay.DayOfWeek) - 1;
            return lastThousandDay.AddDays(days * -1).ToString("yyyy-MM-dd");
        }

        private string ROE大於15且股價小於50(string datetime)
        {
            return $@"
SELECT s.*
  FROM [StockDb].[dbo].[Stocks] s
  join (select * from Prices where [Datetime] = '{datetime}') a on a.StockId = s.StockId
  where ROE>15 and a.[Close] < 100 and ROE /ROA <3
  order by a.[Close] 
";
        }

        private string GetWeekAnalyst(string stockId, string datetime, bool chkDate)
        {
            var sql =  $@"DECLARE @MaxDate AS DATETIME = '{datetime}';
DECLARE @stockid AS nvarchar(10) = '{stockId}';
select
	ROW_NUMBER() over (order by [Datetime] desc) as RowNo,
	StockId,
	[Name],
	CONVERT(nvarchar,[Datetime], 23) as [Datetime],
	[PUnder100],
	[SUnder100],
	[POver1000],
	[SOver1000],
	[P200] + [P400] as [PUnder400],
	[S200] + [S400] as [SUnder400],
	[P600] + [P800] + [P1000] as [POver400],
	[S600] + [S800] + [S1000] as [SOver400]
into #t3
from [Thousand] t 
where [StockId] = @stockid and [Datetime] <= @MaxDate and [Datetime] >= DATEADD(DD,-120,@MaxDate)

select 
    newid() as Id,
    t.StockId,
	t.Name,
	t.[Datetime],
	t.[PUnder100],
	cast(t.[SUnder100] - t1.[SUnder100] as int) as [SUnder100],
	t.[POver1000],
	cast(t.[SOver1000] - t1.[SOver1000] as int) as [SOver1000],
	t.[PUnder400],
	cast(t.[SUnder400] - t1.[SUnder400] as int) as [SUnder400],
	t.[POver400],
	cast(t.[SOver400] - t1.[SOver400] as int) as [SOver400],
	p.[Close]
from #t3 t 
join #t3  t1 on t.RowNo +1 = t1.RowNo 
join (select * from [Prices] where StockID = @stockid) p on p.StockId = t.StockId and p.[Datetime] = t.[Datetime]
order by t.[Datetime] desc
drop table #t3
";
            return sql;
        }

        private string GetWeekAnalystOld(string stockId, string datetime)
        {
            return $@"

DECLARE @MaxDate AS DATETIME = '{datetime}';
DECLARE @stockid AS nvarchar(10) = '{stockId}';

WITH cte
AS
(
    SELECT
        [Datetime], 
		DATEDIFF(DAY,  [Datetime], @MaxDate) AS NoDays,
        DATEDIFF(DAY,  [Datetime], @MaxDate)/7 AS NoGroup,
        [外資買賣超],  [投信買賣超] , [自營商買賣超] , ([融資買進] - [融資賣出]) as 融資買賣超, 成交量
    FROM [Prices]
	where StockID = @stockid and [Datetime] <= @MaxDate
)

SELECT  
    DATEADD(DAY, NoGroup*-7, @MaxDate) AS [Datetime],
    SUM([外資買賣超]) as [外資買賣超],
	SUM([投信買賣超]) as [投信買賣超],
	SUM([自營商買賣超]) as [自營商買賣超],
	SUM(融資買賣超) as 融資買賣超,
    SUM(成交量) as 成交量
into #t1
FROM cte 
GROUP BY NoGroup


select 
	StockId,
	[Name],
	[Datetime],
	[Close],
	cast([五日主力買超張數]-[五日主力賣超張數] as int)  as [主力買賣超],
	[董監持股]
into #t2
from [Prices]
where [StockId] = @stockid and [Datetime] <= @MaxDate

select
	ROW_NUMBER() over (order by [Datetime] desc) as RowNo,
	StockId,
	[Name],
	[Datetime],
	[PUnder100],
	[SUnder100],
	[POver1000],
	[SOver1000],
	[P200] + [P400] as [PUnder400],
	[S200] + [S400] as [SUnder400],
	[P600] + [P800] + [P1000] as [POver400],
	[S600] + [S800] + [S1000] as [SOver400]
into #t3
from [Thousand] t 
where [StockId] = @stockid and [Datetime] <= @MaxDate

select 
	t.[Datetime],
	t.[PUnder100],
	cast(t.[SUnder100] - t1.[SUnder100] as int) as [SUnder100],
	t.[POver1000],
	cast(t.[SOver1000] - t1.[SOver1000] as int) as [SOver1000],
	t.[PUnder400],
	cast(t.[SUnder400] - t1.[SUnder400] as int) as [SUnder400],
	t.[POver400],
	cast(t.[SOver400] - t1.[SOver400] as int) as [SOver400]
into #t4
from #t3 t 
join #t3  t1 on t.RowNo +1 = t1.RowNo 

select
    newid() as Id,
	#t2.StockId,
	#t2.[Name],
    CONVERT(nvarchar,#t1.[Datetime], 23) as [Datetime],
	#t4.[PUnder100],
	#t4.[SUnder100],
	#t4.[POver1000],
	#t4.[SOver1000],
	#t4.[PUnder400],
	#t4.[SUnder400],
	#t4.[POver400],
	#t4.[SOver400],
	#t1.[外資買賣超],
    #t1.[融資買賣超],
    #t1.[投信買賣超],
    #t1.[自營商買賣超],
    #t2.[主力買賣超],
    #t2.[Close],
    #t2.[董監持股],
    #t1.[成交量]
from #t1 
join #t2 on #t1.Datetime = #t2.Datetime
join #t4 on #t1.Datetime = #t4.Datetime
order by #t1.Datetime desc

drop table #t1, #t2, #t3, #t4
";
        }
        Task<Stocks[]> IStockQueries.GetActiveStocksAsync()
        {
            var context = new StockDbContext();
            return context.Stocks
                .Where(p => p.Status == 1)
                .ToArrayAsync();
        }

        Task<Stocks[]> IStockQueries.GetStocksBySqlAsync(string sql)
        {
            var context = new StockDbContext();
            return context.Stocks
                .FromSqlRaw(sql)
                .ToArrayAsync();
        }

        Task<Stocks[]> IStockQueries.GetStocksByTypeAsync(string type)
        {
            var context = new StockDbContext();
            return (from b in context.BestStocks join
                     s in context.Stocks on b.StockId equals s.StockId
                       where b.Type == type select s
                      ).OrderBy(p => p.CreatedOn).ToArrayAsync();
        }

        Task<string[]> IStockQueries.GetChosenStockTypesAsync()
        {
            var context = new StockDbContext();
            return context.BestStocks.Select(p => p.Type)
                .Distinct()
                .OrderBy(p=>p)
                .ToArrayAsync();
        }

        async Task IStockQueries.SetBestStockAsync(string stockId, string type)
        {
            var context = new StockDbContext();
            var stock = context.BestStocks.FirstOrDefault(p => p.StockId == stockId && p.Type == type);
            if (stock != null)
            {
                var best = new BestStocks
                {
                    Id = Guid.NewGuid(),
                    StockId = stock.StockId,
                    Name = stock.Name,
                    Type = type,
                    CreatedOn = DateTime.Now
                };
                context.BestStocks.Add(best);
                await context.SaveChangesAsync();
            }
        }

        async Task<string[]> IStockQueries.GetDaysAsync()
        {
            var context = new StockDbContext();
            var datetimes = await  context.Prices.Where(p=>p.StockId == "1101")
                .GroupBy(p => p.Datetime)
                .OrderByDescending(p => p.Key)
                .Take(220)
                .Select(p=>p.Key.ToString("yyyy-MM-dd"))
                .ToListAsync();
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            if (datetimes.FirstOrDefault() != today && !(DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday))
            {
                datetimes.Insert(0, today);
            }
            return datetimes.ToArray();
        }

        Task<Stocks[]> IStockQueries.GetBestStocksAsync(int key)
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.FromSqlRaw(MapFunc[key]());
            return stocks.ToArrayAsync();
        }

        private string Get淨值比小於2AndROE大於10()
        {
            return $@"
select * from [Stocks] 
where [股價] / [ROE] < 2 and  [股價] / [每股淨值]  <2
and [每股淨值] > 0 and ROE>=10 
order by [股價]
";
        }

        private string Get投信突然加入買方Sql(string datetime)
        {
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(5)
                .OrderBy(p => p.Datetime)
                .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
                .FirstOrDefault();

            return $@"

  WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and [Datetime] >= '{datetime2}'
)

select s.*
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
order by t1.[Close]  
";
        }

        private string Get半年線附近Sql(string datetime, string orderby = "")
        {
            return $@"
select top 200 s.* 
from [Prices] p join [Stocks] s on s.StockId = p.StockId 
where [Datetime] =  '{datetime}' and ([Close] - [MA120])/[Close] > -0.05
order by  abs([Close] - [MA120])/[Close]
";
        }
        private string Get大戶增加散戶減少Sql(string datetime, string orderby = "")
        {
            return $@"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Thousand] where [Datetime] <= '{datetime}' and [POver1000] <= [PPOver1000]-- and [PUnder100] >= [PPUnder100]
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from [Thousand] a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime] 
group by a.StockId,a.Name 
having count(1) >= 1
order by count(1) desc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,s.股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp
";
        }

        private string Get當週大戶比例增加(string datetime)
        {
            return $@"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Thousand] where [Datetime] <= '{datetime}'
)

select *, (POver1000 - PPOver1000) as [Percent] 
into #tmp
from [TOPTEN]
where RowNo = 1 and (POver1000 - PPOver1000) > 0.5 and (PUnder100 < PPUnder100) 

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
	  ,CAST(t.[Percent] AS nvarchar(30)) AS [Description]
      ,s.股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by  (t.POver1000 - t.PPOver1000) desc

drop table #tmp
";
        }

        private string 突然進前20名(string datetime, string stype)
        {
            return $@"declare @date as Datetime = '{datetime}';
SELECT *, ROW_NUMBER() 
over (
    PARTITION BY [Datetime]
    order by [{stype}] * [Close] desc
) AS RowNo 
into #t1
FROM [Prices] where [Datetime] >= DATEADD(DD, -7, @date) and [Datetime] <=  DATEADD(DD, -1, @date)

select s.* from ( 
select top 20 * from [Prices]
where [Datetime] = @date order by [{stype}]*[Close] desc) a  
join Stocks s on s.StockId = a.StockId
where a.StockId not in (select StockId from #t1 where RowNo <= 20)

drop table #t1";
        }

        private string 投量比加主力買超(string datetime)
        {
            return $@"select s.[StockId]
      ,s.[Name]
      ,[MarketCategory]
      ,[Industry]
      ,[ListingOn]
      ,s.[CreatedOn]
      ,[UpdatedOn]
      ,[Status]
      ,[Address]
      ,[Website]
      ,[營收比重]
      ,[股本]
      ,[股價]
      ,[每股淨值]
      ,[每股盈餘]
      ,[ROE]
      ,[ROA]
      ,[股票期貨]
	  ,CAST(round(100* [投信買賣超]/ cast([成交量] as decimal(11)), 10)  AS varchar(18)) AS [Description] FROM [dbo].[Prices] p join [dbo].[Stocks] s on s.StockId = p.StockId
  where [Datetime] = '{datetime}' and [投信買賣超] > 0 and 主力買超張數 - 主力賣超張數 > 0 and [成交量] > 0
  order by [投信買賣超]/ cast([成交量] as decimal) desc";
        }

        private string 每周投信買散戶賣(StockDbContext context, string datetime)
        {
            var today = Convert.ToDateTime(datetime);
            
            var lastThousandDay = context.Thousand.Where(p=>p.StockId == "2330" && p.Datetime <= today)
                .OrderByDescending(p=>p.Datetime)
                .Select(p => p.Datetime)
                .FirstOrDefault();
            var lastMonday = GetLastMonday(lastThousandDay);

            var res = $@"select 
    ss.[StockId]
	,ss.[Name]
	,ss.[MarketCategory]
	,ss.[Industry]
	,ss.[ListingOn]
	,ss.[CreatedOn]
	,ss.[UpdatedOn]
	,ss.[Status]
	,ss.[Address]
	,ss.[Website]
	,ss.[營收比重]
	,ss.[股本]
	,ss.[股價]
	,ss.[每股淨值]
	,ss.[每股盈餘], ss.[ROE], ss.[ROA]
	,CAST(st.[投信買賣超] AS nvarchar(30)) AS [Description] 
	,股票期貨
from 
[Stocks] ss 
join
	(select 
	   p.StockId, p.Name, 
	   sum(p.投信買賣超) as 投信買賣超,
	   sum(p.外資買賣超) as 外資買賣超,
	   sum(p.主力買超張數 - p.主力賣超張數) as 主力買賣超
	from [Prices] p
		join [Stocks] s on s.StockId = p.StockId
		join [Thousand] th on th.StockId=p.StockId
	where 
		p.[Datetime] >= '{lastMonday}' and  p.[Datetime] <= '{lastThousandDay.ToString("yyyy-MM-dd")}' 
		and s.股本 < 100 
		and s.[股價] < 100
		and th.[Datetime] = '{lastThousandDay.ToString("yyyy-MM-dd")}' and th.PUnder100 < th.PPUnder100 and th.POver1000 > th.PPOver1000
	group by p.StockId, p.Name
	having sum(p.投信買賣超) > 0 and sum(p.外資買賣超) > 0 and sum(p.主力買超張數 - p.主力賣超張數) > 0)
	st on ss.StockId = st.StockId
order by st.[投信買賣超] desc";
            return res;
        }

        private string 連續兩天漲停板(StockDbContext context, string datetime)
        {
            var today = Convert.ToDateTime(datetime);

            var lastDays = context.Prices.Where(p => p.StockId == "2330" && p.Datetime < today)
                .OrderByDescending(p => p.Datetime)
                .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
                .Take(2).ToArray();

            var res = $@"select s.* from 
Stocks s 
join
	(select * from [Prices] where [Datetime] = '{datetime}') a on s.StockId = a.StockId
join 
	(select * from [Prices] where [Datetime] = '{lastDays[0]}') b on a.StockId = b.StockId
--join 
--	(select * from [Prices] where [Datetime] = '{lastDays[1]}') c on a.StockId = c.StockId
where a.漲跌百分比 > 9.4 and b.漲跌百分比 > 9.4 and a.[Close] > 10 and a.[Close] < 180 order by a.[Close] desc";
            return res;
        }

        private string 連續上漲天數(string datetime)
        {
            var lastday = Convert.ToDateTime(datetime);
            var last5days = Convert.ToDateTime(lastday).AddMonths(-3).ToString("yyyy-MM-dd");

            return $@"WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] >= '{last5days}' and [Datetime] <= '{datetime}' and [漲跌] < 0
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from Prices a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime] and a.[Datetime] >= '{last5days}' and a.[Datetime] <= '{datetime}'
group by a.StockId,a.Name 
having count(1) >= 3
order by count(1) desc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private async Task<string> 五日漲幅排行榜(StockDbContext context, string datetime, int days)
        {
            var date = Convert.ToDateTime(datetime);
            var dd = await context.Prices.Where(p=>p.StockId=="2330" && p.Datetime<= date)
                .Select(p => p.Datetime)
                .OrderByDescending(p => p).Take(days)
                .OrderBy(p => p)
                .FirstOrDefaultAsync();

            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name]
       order by [CreatedOn] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime]>= '{dd.ToString("yyyy-MM-dd")}' and [Datetime] <= '{datetime}'
)

SELECT top 100 StockId, Name, sum(漲跌百分比) as [Description]
into #tmp
FROM TOPTEN 
WHERE RowNo <= {days}
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
drop table #tmp";
        }

        private string 五日跌幅排行榜(string datetime, int days)
        {
            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [CreatedOn] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}'
)

SELECT top 100 StockId, Name, sum(漲跌百分比) as [Description]
into #tmp
FROM TOPTEN 
WHERE RowNo <= {days}
Group by StockId, Name
order by sum(漲跌百分比) asc


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
order by t.[Description] asc
drop table #tmp";
        }

        private string 連續買超排行榜(string datetime, string 買賣超 = "投信買賣超")
        {
            var pastSeasonDate = Convert.ToDateTime(datetime).AddMonths(-3).ToString("yyyy-MM-dd");
            return @$"WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] >= '{pastSeasonDate}' and [Datetime] <= '{datetime}' and [{買賣超}]<=0
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from Prices a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime] and a.[Datetime] >= '{pastSeasonDate}' and a.[Datetime] <= '{datetime}'
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) desc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private string 每周成交量增長排行榜(string datetime)
        {
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(9)
                .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
                .ToArray();

            return $@"
select s.* 
from
(select 
	p.StockId,
	p.Name,
	Sum(p.投信買賣超) as 投信買賣超,
	Sum(t.Vol2) as 成交量2,
	sum(p.成交量) as 成交量
from Prices p
join [Stocks] s on s.StockId = p.StockId
cross apply (select 成交量 as Vol2 from Prices where StockId = p.StockId and [Datetime] >= '{datetime2[8]}' and [Datetime] < '{datetime2[3]}') as t
where p.[Datetime] <= '{datetime}' and p.[Datetime] >= '{datetime2[3]}'
group by p.StockId, p.Name) a join [Stocks] s on s.StockId = a.StockId
where a.成交量 > (a.成交量2 * 2) and a.投信買賣超 > 0 and s.[股價] < 150 and  s.[股價] > 10
order by a.成交量 / a.成交量2 desc
";
        }

        private string 真主力連續買超排行榜(string datetime, bool 買超 = true)
        {
            var key = 買超 ? "<" : ">";
            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and ([主力買超張數] - [主力賣超張數]) {key}=0
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from Prices a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime]  and a.[Datetime] <= '{datetime}'
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) desc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private string 融資連續買超排行榜(string datetime, bool 買超 = true)
        {
            var key = 買超 ? "<" : ">";
            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and ([融資買進] - [融資賣出]) {key}=0
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from Prices a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime]  and a.[Datetime] <= '{datetime}'
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) desc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private string 主力連續買超排行榜(string datetime, string 買賣超 = "投信買賣超")
        {
            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and  [{買賣超}]<0
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from Prices a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime] and a.[Datetime] <= '{datetime}'
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) desc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private string 連續賣超排行榜(string datetime, string 買賣超 = "投信買賣超")
        {
            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and  [{買賣超}]>=0
)

select 
a.StockId,
a.Name, 
count(1) as [Count]
into #tmp
from Prices a join (
	SELECT 	*
	FROM TOPTEN 
	WHERE RowNo <= 1) b on a.StockId = b.StockId
where a.[Datetime] > b.[Datetime] and a.[Datetime] <= '{datetime}'
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) asc

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
      ,股票期貨
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private string 連續兩週大戶增散戶減(string datetime)
        {
            return $@"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Thousand] where [Datetime] <= '{datetime}'
)

select s.*
from [Stocks]s 
join TOPTEN t1 on s.StockId = t1.StockId
join TOPTEN t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
WHERE t1.RowNo=1 and t1.[POver1000] >  t2.[POver1000] 
and  t1.[PUnder100] <  t2.[PUnder100]
and t2.[POver1000] >  t3.[POver1000] 
and  t2.[PUnder100] <  t3.[PUnder100]
order by  (t1.[POver1000] -  t2.[POver1000]) desc 
";
        }

        private string 近月營收累積年增率成長(string datetime)
        {
            var d = Convert.ToDateTime(datetime).AddMonths(-1).ToString("yyyy-MM-01");
            return $@"
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
	  ,CAST(m.[累積年增率] AS nvarchar(30)) AS [Description] 
      ,股票期貨
  from [MonthData] m
  join [Stocks] s on s.StockId = m.StockId
  where m.[Datetime] = '{d}'
  order by [累積年增率] desc
";
        }
        private string 連續十二月單月年增率成長(string datetime)
        {
            return $@"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [MonthData] where [Datetime] <= '{datetime}'
)

select s.*
from [Stocks]s 
join TOPTEN t1 on s.StockId = t1.StockId
join TOPTEN t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
join TOPTEN t4 on t1.StockId = t4.StockId and t1.RowNo + 3 = t4.RowNo
join TOPTEN t5 on t1.StockId = t5.StockId and t1.RowNo + 4 = t5.RowNo
join TOPTEN t6 on t1.StockId = t6.StockId and t1.RowNo + 5 = t6.RowNo
join TOPTEN t7 on t1.StockId = t7.StockId and t1.RowNo + 6 = t7.RowNo
join TOPTEN t8 on t1.StockId = t8.StockId and t1.RowNo + 7 = t8.RowNo
join TOPTEN t9 on t1.StockId = t9.StockId and t1.RowNo + 8 = t9.RowNo
join TOPTEN t10 on t1.StockId = t10.StockId and t1.RowNo + 9 = t10.RowNo
join TOPTEN t11 on t1.StockId = t11.StockId and t1.RowNo + 10 = t11.RowNo
join TOPTEN t12 on t1.StockId = t12.StockId and t1.RowNo + 11 = t12.RowNo

WHERE t1.RowNo=1 
and t1.單月年增率 > 0
and t2.單月年增率 > 0
and t3.單月年增率 > 0
and t4.單月年增率 > 0
and t5.單月年增率 > 0
and t6.單月年增率 > 0
and t7.單月年增率 > 0
and t8.單月年增率 > 0
and t9.單月年增率 > 0
and t10.單月年增率 > 0
and t11.單月年增率 > 0
and t12.單月年增率 > 0
order by s.[Description] / s.每股淨值
";
        }

        private Dictionary<ChooseStockType, Func<string>> DateFunc = new Dictionary<ChooseStockType, Func<string>>
        {
            { ChooseStockType.一日漲幅排行榜 , ()=>一日漲幅排行榜() },
            { ChooseStockType.外資投信同步買超排行榜 , ()=>外資投信同步買超排行榜() },
            { ChooseStockType.外資主力同步買超排行榜 , ()=>外資主力同步買超排行榜() },
            { ChooseStockType.外資買超排行榜  , ()=>外資買超排行榜() },
            { ChooseStockType.投信買超排行榜  , ()=>投信買超排行榜() },
            { ChooseStockType.自營買超排行榜  , ()=>自營買超排行榜() },
            { ChooseStockType.融資買超排行榜  , ()=>融資買超排行榜() },
            { ChooseStockType.融券賣超排行榜  , ()=>融券賣超排行榜() },
            
            { ChooseStockType.一日跌幅排行榜  , ()=>一日跌幅排行榜() },
            { ChooseStockType.外資投信同步賣超排行榜  , ()=>外資投信同步賣超排行榜() },
            { ChooseStockType.投信連續賣超排行榜 , ()=>外資投信同步賣超排行榜() },
            { ChooseStockType.外資賣超排行榜  , ()=>外資賣超排行榜() },
            { ChooseStockType.投信賣超排行榜  , ()=>投信賣超排行榜() },
            { ChooseStockType.自營賣超排行榜  , ()=>自營賣超排行榜() },
            { ChooseStockType.融資賣超排行榜  , ()=>融資賣超排行榜() },
            { ChooseStockType.融券買超排行榜  , ()=>融券買超排行榜() },
            { ChooseStockType.漲停板  , ()=>漲停板() },
            { ChooseStockType.當沖比例 , ()=>當沖比例()},
            { ChooseStockType.當沖總損益 , ()=>當沖總損益()},
            { ChooseStockType.當沖均損益 , ()=>當沖均損益()}
        };

        private Dictionary<int, Func<string>> MapFunc = new Dictionary<int, Func<string>>
        {
            { 0 , ()=>GetActiveStocksSql() },
            { 1 , ()=>GetSqlByFinance() },
            { 2 , ()=>GetSqlByShape() },
            { 3 , ()=>GetSqlByChoose() },
            { 6 , ()=>GetFutureEngingStocksSql()}
        };

        #region 買賣超排行榜

        private static string 當沖比例()
        {
            return @$"
  and [當沖比例] > 1
  order by [當沖比例] desc
";
        }
        private static string 當沖總損益()
        {
            return @$"
  order by [當沖總損益] desc
";
        }

        private static string 當沖均損益()
        {
            return @$"
  order by [當沖均損益] desc
";
        }

        private static string 一日漲幅排行榜()
        {
            return @$"
  and [漲跌百分比] > 2
  order by [漲跌百分比] desc
";
        }

        private static string 一日跌幅排行榜()
        {
            return @$"
  and [漲跌百分比] < -2
  order by [漲跌百分比] asc
";
        }

        private static string 外資投信同步買超排行榜()
        {
            return @$"
  and [外資買賣超] > 0 and [投信買賣超] > 0 and ([主力買超張數] - [主力賣超張數]) > 0 and [漲跌百分比] > 2
  order by [投信買賣超] desc
";
        }

        private static string 外資主力同步買超排行榜()
        {
            return @$"
  and [外資買賣超] > 0 and ([主力買超張數] - [主力賣超張數])> 0
  order by [投信買賣超] desc
";
        }
        
        private static string 外資投信同步賣超排行榜()
        {
            return @$"
  and [外資買賣超] < 0 and [投信買賣超] < 0
  order by [投信買賣超] asc
";
        }

        private static string 外資買超排行榜()
        {
            return @$"
  and [外資買賣超] > 0
  and [外資買賣超] * [Close] > 4000
  order by [外資買賣超] desc
";
        }

        private static string 外資賣超排行榜()
        {
            return @$"
  and [外資買賣超] < 0
  and [外資買賣超] * [Close] < -4000
  order by [外資買賣超] asc
";
        }

        private static string 投信買超排行榜()
        {
            return @$"
  and [投信買賣超] > 0
  order by [投信買賣超] desc
";
        }

        private static string 投信賣超排行榜()
        {
            return @$"
  and [投信買賣超] < 0
  order by [投信買賣超] asc
";
        }

        private static string 自營買超排行榜()
        {
            return @$"
  and [自營商買賣超] > 0
  and [自營商買賣超] * [Close] > 1000
  order by [自營商買賣超] desc
";
        }

        private static string 自營賣超排行榜()
        {
            return @$"
  and [自營商買賣超] < 0
  and [自營商買賣超] * [Close] <- 1000
  order by [自營商買賣超] asc
";
        }


        private static string 融資買超排行榜()
        {
            return @$"
  and ([融資買進] - [融資賣出])>0
  and ([融資買進] - [融資賣出]) * [Close] > 5000
  order by ([融資買進] - [融資賣出]) desc 
";
        }

        private static string 融資賣超排行榜()
        {
            return @$"
  and ( [融資賣出] - [融資買進])>0
  and ( [融資賣出] - [融資買進]) * [Close] > 5000
  order by ([融資賣出] - [融資買進]) desc 
";
        }

        private static string 融券買超排行榜()
        {
            return @$"
  and ([融券買進] - [融券賣出])>0
  order by ([融券買進] - [融券賣出]) desc 
";
        }

        private static string 融券賣超排行榜()
        {
            return @$"
  and ( [融券賣出] - [融券買進])>0
  order by ( [融券賣出] - [融券買進])  desc 
";
        }

        private static string 漲停板()
        {
            return @$"
  and [漲跌百分比] >= 9.65
  order by [漲跌百分比] desc 
";
        }

        #endregion

        #region 選股
        private static string GetSqlByFinance()
        {
            return @"
select * from [Stocks] 
where StockId in (
  select StockId from [BestStocks] 
  where [Type] = '財報選股'
)
order by StockId
";
        }

        private static string GetSqlByShape()
        {
            return @"
select * from [Stocks] 
where StockId in (
  select StockId from [BestStocks] 
  where [Type] = '型態選股'
)
order by StockId
";
        }

        private static string GetSqlByChoose()
        {
            return @"
select * from [Stocks] 
where StockId in (
  select StockId from [BestStocks] 
  where [Type] = '頁面選股'
)
order by StockId
";
        }

        private static string GetActiveStocksSql()
        {
            return @"
select * from [Stocks] 
where [Status] = 1
order by StockId
";
        }

        private static string GetFutureEngingStocksSql()
        {
            return @"
select * from [Stocks]  
where StockId in (
SELECT
       [StockId]
  FROM [StockDb].[dbo].[AnaFutureEngine]
    where [Pass] = 1
  group by    [StockId]
      ,[Name]
	having count([Pass]) >=10)
";
        }

        private string Get多頭排列SQL(string datetime)
        {
            _context = new StockDbContext();
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(1)
                .Select(p => p.Datetime)
                .FirstOrDefault().ToString("yyyy/MM/dd");

            return $@"select s.* from Stocks s join (SELECT *
  FROM [StockDb].[dbo].[Prices]
  where [Datetime] = '{datetime}' and MA5 > MA10 and MA10 > MA20 and MA20 > MA60) a1 on s.StockId = a1.StockId
  join  (SELECT *
  FROM [StockDb].[dbo].[Prices]
  where [Datetime] = '{datetime2}' and not (MA5 > MA10 and MA10 > MA20 and MA20 > MA60)) a2 on a1.StockId = a2.StockId
  order by s.StockId";
        }

        private string Get上漲破五日均SQL(string datetime)
        {
            _context = new StockDbContext();
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(1)
                .Select(p => p.Datetime)
                .FirstOrDefault().ToString("yyyy-MM-dd");

            return $@"select s.* 
  from [Stocks] s 
    join
	  (select * 
	  from [Prices] with (nolock)
	  where [Datetime] = '{datetime}') a on s.StockId = a.StockId
	join
	  (select * 
	  from [Prices]
	  where [Datetime] = '{datetime2}') b on a.StockId = b.StockId
  where a.[漲跌百分比] > 2.5
	  and a.[Close] > a.MA5 and a.[open] < a.MA5
	  and a.[Close] > 20 and a.[Close] < 170
	  and a.[Close] > b.[Open]
--      and (a.投信買賣超 > 0 or b.投信買賣超 > 0)
  order by a.[漲跌百分比] desc";
        }

        private string Get上漲破月線SQL(string datetime)
        {
            _context = new StockDbContext();
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(1)
                .Select(p => p.Datetime)
                .FirstOrDefault().ToString("yyyy-MM-dd");

            return $@"select s.* from Stocks s join 
(select * from [Prices] a where a.Datetime = '{datetime}') a1 on s.StockId = a1.StockId join 
(select * from [Prices] a where a.Datetime = '{datetime2}') a2 on a1.StockId = a2.StockId
where a1.[Close] > a1.MA20 
	and a1.[Close] > a1.MA60 
	and a2.[Close] < a2.MA20 
	and a1.漲跌百分比 > 3 
	and a1.成交量 > a2.VMA5 * 2
	and ((a1.主力買超張數 - a1.主力賣超張數) / a1.成交量)  > 0.01
	and a1.成交量 > 0
	and a1.[Close] > a1.[Open]
--	and a2.十日主力買超張數 > a2.十日主力賣超張數
--	and a2.二十日主力買超張數 > a2.二十日主力賣超張數
order by a1.StockId
";
        }
        private string Get三天漲百分之二十SQL(string datetime)
        {
            _context = new StockDbContext();
            var dd = Convert.ToDateTime(datetime);
            var datetimeList = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(2)
                .Select(p => p.Datetime.ToString("yyyy/MM/dd")).ToArray();
            var datetime2 = datetimeList[1];
            return $@"WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and [Datetime] >= '{datetime2}'
)

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
	  ,CAST(a.[漲跌百分比] AS nvarchar(30)) AS [Description]
      ,股票期貨 from [Stocks] s
join (
select StockId, [Name], sum([漲跌百分比]) as [漲跌百分比] from TOPTEN
group by StockId, [Name]
having sum([漲跌百分比]) > 20) a on s.StockId = a.StockId
order by a.漲跌百分比 desc";
        }

        private string Get三天漲百分之二十且投信買超SQL(string datetime)
        {
            _context = new StockDbContext();
            var dd = Convert.ToDateTime(datetime);
            var datetimeList = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(2)
                .Select(p => p.Datetime.ToString("yyyy/MM/dd")).ToArray();
            var datetime2 = datetimeList[1];
            return $@"WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and [Datetime] >= '{datetime2}'
)

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
	  ,CAST(a.[漲跌百分比] AS nvarchar(30)) AS [Description]
      ,股票期貨 from [Stocks] s
join (
select StockId, [Name], sum([漲跌百分比]) as [漲跌百分比] from TOPTEN
group by StockId, [Name]
having sum([漲跌百分比]) > 20 and  sum([投信買賣超]) > 0) a on s.StockId = a.StockId
order by a.漲跌百分比 desc";
        }

        private string GetSqlByChairmanAsync(string datetime)
        {
            _context = new StockDbContext();
            var dd = Convert.ToDateTime(datetime);
            var datetime2 = _context.Prices.Where(p => p.StockId == "2330" && p.Datetime < dd)
                .OrderByDescending(p => p.Datetime)
                .Take(1)
                .Select(p=>p.Datetime)
                .FirstOrDefault().ToString("yyyy/MM/dd");

            return $@"
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
where a.[董監持股] !=  b.[董監持股]) b on s.StockId = b.StockId
order by b.買超 desc
";
        }

        Task<BestStockType[]> IStockQueries.GetBestStockTypeAsync()
        {
            var context = new StockDbContext();
            return context.BestStockType.OrderBy(p => p.Key).ToArrayAsync();
        }

        async Task<Stocks[]> IStockQueries.GetStocksByBestStockTypeAsync(string name, string datetime)
        {
            var date = Convert.ToDateTime(datetime);
            var context = new StockDbContext();
            return await (
            from s in context.Stocks
            join best in context.RealtimeBestStocks on s.StockId equals best.StockId
            where best.Datetime == date && best.Type == name
            orderby s.StockId
            select s).ToArrayAsync();
        }

        Task<string> IStockQueries.GetTokenAsync()
        {
            var context = new StockDbContext();
            return context.Token.Select(p=>p.LineToken).FirstOrDefaultAsync();
        }

        async Task<string[]> IStockQueries.GetMinuteKLinesAsync()
        {
            var s = new[] 
            {
                "最近十天每三天漲幅超過20%",
                "最近十天每天外資投信主力買超",
                "最近十天每天外資投信主力買超"
            };
            var context = new StockDbContext();
            var result = new List<string>();
            var minutes = new[] { 5, 10, 30 };

            for (int i = 0; i < minutes.Length; i++)
            {
                var key = $"_{minutes[i]}分K線均線多排";
                var stocks = await context.BestStocks.Where(p => p.Type == key)
                    .OrderBy(p => p.StockId)
                    .Select(p=>p.StockId)
                    .ToArrayAsync();

                string s1 = string.Join(',', stocks).Replace(" ","");
                result.Add($"{key}={s1}");
            }
            return result.ToArray();       
        }
        #endregion
    }
}

//連續外資買超天數
//WITH TOPTEN as (
//   SELECT*, ROW_NUMBER()
//   over(
//       PARTITION BY  StockId, Name
//      order by[Datetime] desc
//   ) AS RowNo
//    FROM[Prices] where[Datetime] <= '2019-10-24' and[外資買賣超]<=0
//)

//select
//a.StockId,
//a.Name,
//count(1)
//from Prices a join(
//SELECT
//*
//FROM TOPTEN
//WHERE RowNo <= 1) b on a.StockId = b.StockId
//where a.[Datetime] > b.[Datetime]
//group by a.StockId, a.Name
//order by count(1) desc
