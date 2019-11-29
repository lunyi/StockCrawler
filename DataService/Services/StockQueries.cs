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
        async Task<StockeModel> IStockQueries.GetPricesByStockIdAsync(string stockId)
        {
            var context = new StockDbContext();
            var prices = await (from price in context.Prices
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
                         外資持股 = price.外資持股,
                         投信持股 = price.投信持股,
                         董監持股 = price.董監持股,
                         外資持股比例 = price.外資持股比例,
                         融資買賣超 = price.融資買進 - price.融資賣出,
                         融資使用率 = price.融資使用率,
                         外資持股比重 = price.外資持股比例,
                         外資買賣超 = price.外資買進 - price.外資賣出,
                         投信買賣超 = price.投信買進 - price.投信賣出,
                         自營商買賣超 = price.自營商買進 - price.自營商賣出,
                         主力買賣超 = price.主力買超張數 - price.主力賣超張數,
                         籌碼集中度 = 100 * Math.Round(((price.主力買超張數 - price.主力賣超張數) / price.成交量).Value, 5),
                         周轉率 = 100 * Math.Round(((decimal)price.成交量 / price.發行張數).Value, 5)
                     }).Take(60).ToArrayAsync() ;
            return new StockeModel
            {
                Stock = await context.Stocks.FirstOrDefaultAsync(p=>p.StockId == stockId),
                Prices = prices
            };
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

        Task<string[]> IStockQueries.GetDaysAsync()
        {
            var context = new StockDbContext();
            return  context.Prices
                .GroupBy(p => p.Datetime)
                .OrderByDescending(p => p.Key)
                .Take(40)
                .Select(p=>p.Key.ToString("yyyy-MM-dd"))
                .ToArrayAsync();
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
                case (int)ChooseStockType.買方籌碼集中排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime, "desc");
                    break;
                case (int)ChooseStockType.連續十二月單月年增率成長:
                    sql = 連續十二月單月年增率成長(datetime);
                    break;

                case (int)ChooseStockType.當月大戶增散戶減投信買:
                    sql = Get當月大戶增散戶減投信買Sql(datetime);
                    break;

                case (int)ChooseStockType.賣方籌碼集中排行榜:
                    sql = Get籌碼集中排行榜Sql(datetime);
                    break;
                case (int)ChooseStockType.半年線附近:
                    sql = Get半年線附近Sql(datetime);
                    break;
                case (int)ChooseStockType.大戶增加散戶減少:
                    sql = Get大戶增加散戶減少Sql(datetime);
                    break;
                case (int)ChooseStockType.集保庫存排行榜:
                    sql = 集保庫存排行榜(datetime);
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

        private string Get籌碼集中排行榜Sql(string datetime, string orderby = "")
        {
            return $@"
select top 100
s.*
from [Prices] p join [Stocks] s on s.StockId = p.StockId 
where p.[Datetime] = '{datetime}'
order by (p.[主力買超張數] - p.[主力賣超張數]) / p.成交量 {orderby}
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

WITH TOPTEN1 as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Thousand] where [Datetime] <= '{datetime}'
)

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
	  ,s.[Description]
from [Stocks]s 
join TOPTEN1 t1 on s.StockId = t1.StockId
join TOPTEN1 t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN1 t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
join TOPTEN1 t4 on t1.StockId = t4.StockId and t1.RowNo + 3 = t4.RowNo
join TOPTEN1 t5 on t1.StockId = t5.StockId and t1.RowNo + 4 = t5.RowNo
join TOPTEN1 t6 on t1.StockId = t6.StockId and t1.RowNo + 5 = t6.RowNo
WHERE t1.RowNo=1 and 
(t1.[PercentOverThousand] > t2.[PercentOverThousand]) and 
(t2.[PercentOverThousand] > t3.[PercentOverThousand]) and
(t3.[PercentOverThousand] > t4.[PercentOverThousand]) and
(t4.[PercentOverThousand] > t5.[PercentOverThousand]) and
(t5.[PercentOverThousand] > t6.[PercentOverThousand]) and
(t1.[PercentUnderFourHundreds] < t2.[PercentUnderFourHundreds]) and 
(t2.[PercentUnderFourHundreds] < t3.[PercentUnderFourHundreds]) and
(t3.[PercentUnderFourHundreds] < t4.[PercentUnderFourHundreds]) and
(t4.[PercentUnderFourHundreds] < t5.[PercentUnderFourHundreds]) and
(t5.[PercentUnderFourHundreds] < t6.[PercentUnderFourHundreds]) 
order by StockId;
";
        }

        private string Get當月大戶增散戶減投信買Sql(string datetime)
        {
            var past30Days = Convert.ToDateTime(datetime).AddDays(-30).ToString("yyyy-MM-dd");
            return $@"

WITH TOPTEN1 as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Thousand] where [Datetime] <= '{datetime}'
)

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
	  ,s.[Description]
into #t2
from [Stocks]s 
join TOPTEN1 t1 on s.StockId = t1.StockId
join TOPTEN1 t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN1 t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
join TOPTEN1 t4 on t1.StockId = t4.StockId and t1.RowNo + 3 = t4.RowNo
join TOPTEN1 t5 on t1.StockId = t5.StockId and t1.RowNo + 4 = t5.RowNo
join TOPTEN1 t6 on t1.StockId = t6.StockId and t1.RowNo + 5 = t6.RowNo
join TOPTEN1 t7 on t1.StockId = t7.StockId and t1.RowNo + 6 = t7.RowNo
join TOPTEN1 t8 on t1.StockId = t8.StockId and t1.RowNo + 7 = t8.RowNo
join TOPTEN1 t9 on t1.StockId = t9.StockId and t1.RowNo + 8 = t9.RowNo
join TOPTEN1 t10 on t1.StockId = t10.StockId and t1.RowNo + 9 = t10.RowNo

WHERE t1.RowNo=1 and
 (t1.PercentOver1000 > t2.PercentOver1000) 
 group by 
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
	  ,s.[Description]
 having
 (sum(t1.P1 + t1.P5+ t1.P10+ t1.P15+ t1.P20+ t1.P30+ t1.P40 + t1.P50 + t1.P100)< sum(t2.P1 + t2.P5+ t2.P10+ t2.P15+ t2.P20+ t2.P30+ t2.P40 + t2.P50 + t2.P100));




WITH TOPTEN2 as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [MonthData] where [Datetime] <= '{datetime}'
)

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
	  ,s.Description
into #t3
from [Stocks]s 
join TOPTEN2 t1 on s.StockId = t1.StockId
join TOPTEN2 t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN2 t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
join TOPTEN2 t4 on t1.StockId = t4.StockId and t1.RowNo + 3 = t4.RowNo
join TOPTEN2 t5 on t1.StockId = t5.StockId and t1.RowNo + 4 = t5.RowNo
join TOPTEN2 t6 on t1.StockId = t6.StockId and t1.RowNo + 5 = t6.RowNo
join TOPTEN2 t7 on t1.StockId = t7.StockId and t1.RowNo + 6 = t7.RowNo
join TOPTEN2 t8 on t1.StockId = t8.StockId and t1.RowNo + 7 = t8.RowNo
join TOPTEN2 t9 on t1.StockId = t9.StockId and t1.RowNo + 8 = t9.RowNo
join TOPTEN2 t10 on t1.StockId = t10.StockId and t1.RowNo + 9 = t10.RowNo
join TOPTEN2 t11 on t1.StockId = t11.StockId and t1.RowNo + 10 = t11.RowNo
join TOPTEN2 t12 on t1.StockId = t12.StockId and t1.RowNo + 11 = t12.RowNo

WHERE t1.RowNo=1 
and t1.單月年增率 >= 0;
--and t2.單月年增率 >= 0
--and t3.單月年增率 >= 0
--and t4.單月年增率 >= 0
--and t5.單月年增率 >= 0
--and t6.單月年增率 >= 0
--and t7.單月年增率 > 0
--and t8.單月年增率 > 0
--and t9.單月年增率 > 0
--and t10.單月年增率 > 0
--and t11.單月年增率 > 0
--and t12.單月年增率 > 0


select  p.StockId, p.Name, sum(投信買賣超) as 投信買賣超
into #t1
from [Prices] p
where [Datetime] >= '{past30Days}' and 投信買賣超 != 0
group by 
 p.StockId, p.Name
 having sum(投信買賣超) > 0
order by sum(投信買賣超) desc

select s.*
from #t2 s
join #t1 on s.StockId = #t1.StockId
join #t3 on s.StockId = #t3.StockId
join (SELECT *
  FROM [StockDb].[dbo].[Prices]
  where [Datetime] = '{datetime}') t3 on t3.StockId = s.StockId
order by s.Industry, CONVERT(decimal(10,2), s.Description);


drop table #t1, #t3 , #t2

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
	  ,CAST(t.[Count] AS nvarchar(30)) AS [Description]
from [Stocks]s 
join #tmp t on s.StockId = t.StockId
order by t.[Count] desc

drop table #tmp";
        }

        private string 集保庫存排行榜(string datetime)
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
	  ,CAST(t1.[Percent] -  t2.[Percent] AS nvarchar(30)) AS [Description]
from [Stocks]s 
join TOPTEN t1 on s.StockId = t1.StockId
join TOPTEN t2 on t1.StockId = t2.StockId and t1.RowNo + 1 = t2.RowNo
join TOPTEN t3 on t1.StockId = t3.StockId and t1.RowNo + 2 = t3.RowNo
WHERE t1.RowNo=1 and (t1.[Percent] -  t2.[Percent]) > 1
order by  (t1.[Percent] -  t2.[Percent]) desc
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
	  ,s.Description
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
";
        }

        private Dictionary<ChooseStockType, Func<string>> DateFunc = new Dictionary<ChooseStockType, Func<string>>
        {
            { ChooseStockType.一日漲幅排行榜 , ()=>一日漲幅排行榜() },
            { ChooseStockType.外資投信同步買超排行榜 , ()=>外資投信同步買超排行榜() },
            { ChooseStockType.投信連續買超排行榜 , ()=>外資投信同步買超排行榜() },
            { ChooseStockType.外資買超排行榜  , ()=>外資買超排行榜() },
            { ChooseStockType.投信買超排行榜  , ()=>投信買超排行榜() },
            { ChooseStockType.自營買超排行榜  , ()=>自營買超排行榜() },
            { ChooseStockType.融資買超排行榜  , ()=>融資買超排行榜() },
            { ChooseStockType.融券賣超排行榜  , ()=>融券賣超排行榜() },
            
            { ChooseStockType.一日跌幅排行榜  , ()=>一日跌幅排行榜() },
            { ChooseStockType.外資投信同步賣超排行榜  , ()=>外資投信同步賣超排行榜() },
            { ChooseStockType.投信連續賣超排行榜 , ()=>外資投信同步買超排行榜() },
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
