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
        Task<StockeModel> GetPricesByStockIdAsync(string stockId);
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
select top 300
 s.[Id]
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
      ,s.[每股盈餘]
	  ,s.[Description]
from [Prices] p join [Stocks] s on s.StockId = p.StockId 
where p.[Datetime] = '{datetime}'
order by (p.[{strDays}主力買超張數] - p.[{strDays}主力賣超張數]) / p.{strVolumn} {orderby}
";
        }

        async Task<StockeModel> IStockQueries.GetPricesByStockIdAsync(string stockId)
        {
            var context = new StockDbContext();
            var prices = await (from price in context.Prices
                     join stock in context.Stocks on price.StockId equals stock.StockId
                     where price.StockId == stockId
                     orderby price.Datetime descending
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
                         股價淨值比 = Math.Round(price.Close / stock.每股淨值.Value, 2),
                         投信持股比例 = 100 * Math.Round(((decimal)price.投信持股 / price.發行張數).Value, 5),
                         董監持股比例 = 100 * Math.Round(((decimal)price.董監持股 / price.發行張數).Value, 5),
                         外資持股比例 = price.外資持股比例,
                         融資買賣超 = price.融資買進 - price.融資賣出,
                         融券買賣超 = price.融券買進 - price.融券賣出,
                         融券餘額 = price.融券餘額,
                         董監持股 = price.董監持股,
                         融資使用率 = price.融資使用率,
                         外資買賣超 = price.外資買進 - price.外資賣出,
                         投信買賣超 = price.投信買進 - price.投信賣出,
                         自營商買賣超 = price.自營商買進 - price.自營商賣出,
                         主力買賣超 = price.主力買超張數 - price.主力賣超張數,
                         籌碼集中度 = 100 * Math.Round(((price.主力買超張數 - price.主力賣超張數) / price.成交量).Value, 4),
                         周轉率 = 100 * Math.Round(((decimal)price.成交量 / price.發行張數).Value, 5)
                     }).ToArrayAsync();

            var weeklyChip = await context._WeekyChip.FromSqlRaw(GetWeekAnalyst(stockId, GetLastFriday())).ToArrayAsync();
            var pps = await context.Prices.Where(p => p.StockId == stockId).ToArrayAsync();
            var monthData = await context.MonthData
                .Where(p => p.StockId == stockId)
                .OrderByDescending(p => p.Datetime)
                .Take(12)
                .ToArrayAsync();

            return new StockeModel
            {
                Stock = await context.Stocks.FirstOrDefaultAsync(p=>p.StockId == stockId),
                Prices = prices,
                WeeklyChip = weeklyChip,
                MonthData = monthData
            };
        }

        private string GetLastFriday()
        {
            int days = DateTime.Today.DayOfWeek == DayOfWeek.Saturday ? 1 : 1 * ((int)DateTime.Today.DayOfWeek + 2);
            return DateTime.Today.AddDays(days*-1).ToString("yyyy-MM-dd");
        }
        private string GetWeekAnalyst(string stockId, string datetime)
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
        [外資買賣超],  [投信買賣超] , [自營商買賣超] , ([融資買進] - [融資賣出]) as 融資買賣超
    FROM [Prices]
	where StockID = @stockid and [Datetime] <= @MaxDate
)


SELECT  
    DATEADD(DAY, NoGroup*-7, @MaxDate) AS [Datetime],
    SUM([外資買賣超]) as [外資買賣超],
	SUM([投信買賣超]) as [投信買賣超],
	SUM([自營商買賣超]) as [自營商買賣超],
	SUM(融資買賣超) as 融資買賣超
into #t1
FROM cte 
GROUP BY NoGroup


select 
	StockId,
	[Name],
	[Datetime],
	[Close],
	cast([五日主力買超張數]- [五日主力賣超張數] as int)  as [主力買賣超],
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
    #t2.[董監持股]
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
            var stock = context.Stocks.FirstOrDefault(p => p.StockId == stockId);
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
                .Take(60)
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

       Task<Stocks[]> IStockQueries.GetStocksByDateAsync(string datetime, int type)
        {
            var context = new StockDbContext();

            string sql = string.Empty;

            switch (type)
            {
                case (int)ChooseStockType.五日漲幅排行榜:
                    sql = 五日漲幅排行榜(datetime, 5);
                    break;
                case (int)ChooseStockType.二十日漲幅排行:
                    sql = 五日漲幅排行榜(datetime, 20);
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
                case (int)ChooseStockType.CMoney選股:
                    sql = GetCMoneyStocksSql();
                    break;
                case (int)ChooseStockType.財報狗選股:
                    sql = GetStatemenetDogStocksSql();
                    break;
                default:
                    var whereCondition = DateFunc[(ChooseStockType)type]();
                    sql = @$"SELECT s.*
                          FROM [dbo].[Prices] p
                          join Stocks s on p.StockId = s.StockId
                          where [Datetime] = '{datetime}' {whereCondition}";
                    break;
            }
           
            return context.Stocks.FromSqlRaw(sql).ToArrayAsync();
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
      ,s.[每股盈餘]
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
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
where RowNo = 1 and (POver1000 - PPOver1000) > 0.5

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
      ,s.[每股盈餘]
	  ,CAST(t.[Percent] AS nvarchar(30)) AS [Description]
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by  (t.POver1000 - t.PPOver1000) desc

drop table #tmp
";
        }
        private string 五日漲幅排行榜(string datetime, int days)
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
order by sum(漲跌百分比) desc

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
      ,s.[每股盈餘]
      ,CAST(t.[Description] AS nvarchar(30)) AS [Description]
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
      ,s.[每股盈餘]
      ,CAST(t.[Description] AS nvarchar(30)) AS [Description]
from Stocks s 
join #tmp t on t.StockId = s.StockId
order by t.[Description] asc
drop table #tmp";
        }

        private string 連續買超排行榜(string datetime, string 買賣超 = "投信買賣超")
        {
            return @$"
WITH TOPTEN as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY  StockId, Name 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Prices] where [Datetime] <= '{datetime}' and [{買賣超}]<=0
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
where a.[Datetime] > b.[Datetime] 
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) desc

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
      ,s.[每股盈餘]
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
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
      ,s.[每股盈餘]
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
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
where a.[Datetime] > b.[Datetime] 
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) desc

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
      ,s.[每股盈餘]
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
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
where a.[Datetime] > b.[Datetime] 
group by a.StockId,a.Name 
having count(1) >= 2
order by count(1) asc

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
      ,s.[每股盈餘]
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
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
      ,s.[每股盈餘]
	  ,CAST(m.[累積年增率] AS nvarchar(30)) AS [Description] 
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
            { ChooseStockType.融券買超排行榜  , ()=>融券買超排行榜() }
        };

        private Dictionary<int, Func<string>> MapFunc = new Dictionary<int, Func<string>>
        {
            { 0 , ()=>GetActiveStocksSql() },
            { 1 , ()=>GetSqlByFinance() },
            { 2 , ()=>GetSqlByShape() },
            { 3 , ()=>GetSqlByChoose() },
            { 4 , ()=>GetCMoneyStocksSql() },
            { 5 , ()=>GetStatemenetDogStocksSql() },
            { 6 , ()=>GetFutureEngingStocksSql()}
        };

        #region 買賣超排行榜

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
  and [外資買賣超] > 0 and [投信買賣超] > 0
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

        private static string GetCMoneyStocksSql()
        {
            return @"
  WITH ranked_messages AS (
  SELECT m.*, ROW_NUMBER() OVER (PARTITION BY name ORDER BY CreatedOn DESC) AS rn
  FROM [AnaCMoney] AS m
)

select * from [Stocks] 
where StockId in (SELECT StockId FROM ranked_messages WHERE rn = 1 and Remark in ('很好','優秀'))
order by StockId

";
        }

        private static string GetStatemenetDogStocksSql()
        {
            return @"

select * from [Stocks]  
where StockId in (
select  
a.StockId
from (
	SELECT 
		  c1.[StockId]
		  ,c1.[Name]
		  ,c1.[Type],
		  COUNT(c1.Pass) as TotalCount,
		  COUNT(c2.Pass) as Pass
	  FROM [dbo].[AnaStatementDogs] c1
	  left join (select * from [dbo].[AnaStatementDogs] where Pass = 1) c2 
	  on c1.Id = c2.Id
	  --where 
	  --c2.[Pass] = 1
	  group by  
		  c1.[StockId]
		  ,c1.[Name],
		  c1.[Type] 
) a where a.Pass >= 3

group by 
a.StockId,
a.Name
having count(Pass) = 4
)

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
      ,s.[每股盈餘]
	  ,CAST(b.買超 AS nvarchar(30)) AS [Description]

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
