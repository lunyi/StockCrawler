using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace BlazorApp.Shared
{
    public interface IDataLayer
    {
        Task<Stocks[]> GetBestStocksAsync(int key);
        Task<string[]> GetDaysAsync();
        Task<Stocks[]> GetStocksByDateAsync(string datetime, int type);
    }
    public class DataLayer : IDataLayer
    {
        Task<string[]> IDataLayer.GetDaysAsync()
        {
            var context = new StockDbContext();
            return  context.Prices
                .GroupBy(p => p.Datetime)
                .OrderByDescending(p => p.Key)
                .Take(20)
                .Select(p=>p.Key.ToString("yyyy-MM-dd"))
                .ToArrayAsync();
        }

        Task<Stocks[]> IDataLayer.GetBestStocksAsync(int key)
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.FromSqlRaw(MapFunc[key]());
            return stocks.ToArrayAsync();
        }

        Task<Stocks[]> IDataLayer.GetStocksByDateAsync(string datetime, int type)
        {
            var context = new StockDbContext();
            var whereCondition = DateFunc[type]();
            var sql = @$"SELECT s.*
  FROM [dbo].[Prices] p
  join Stocks s on p.StockId = s.StockId
  where [Datetime] = '{datetime}' {whereCondition}";

            var stocks = context.Stocks.FromSqlRaw(sql);
            return stocks.ToArrayAsync();
        }

        Dictionary<int, Func<string>> DateFunc = new Dictionary<int, Func<string>>
        {
            { 1 , ()=>外資買超排行榜() },
            { 2 , ()=>投信買超排行榜() },
            { 3 , ()=>自營買超排行榜() },
            { 4 , ()=>融資買超排行榜() },
            { 5 , ()=>融券賣超排行榜() },
            { 6 , ()=>外資賣超排行榜() },
            { 7 , ()=>投信賣超排行榜() },
            { 8 , ()=>自營賣超排行榜() },
            { 9 , ()=>融資賣超排行榜() },
            { 10 , ()=>融券賣超排行榜() }
        };

        Dictionary<int, Func<string>> MapFunc = new Dictionary<int, Func<string>>
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

        private static string 外資買超排行榜()
        {
            return @$"
  and [外資買賣超] > 0
  and [外資買賣超] * [Close] > 4000
  order by [外資買賣超] *[Close] desc
";
        }

        private static string 外資賣超排行榜()
        {
            return @$"
  and [外資買賣超] < 0
  and [外資買賣超] * [Close] < -4000
  order by [外資買賣超] *[Close] asc
";
        }

        private static string 投信買超排行榜()
        {
            return @$"
  and [投信買賣超] > 0
  order by [投信買賣超] *[Close] desc
";
        }

        private static string 投信賣超排行榜()
        {
            return @$"
  and [投信買賣超] < 0
  order by [投信買賣超] *[Close] asc
";
        }

        private static string 自營買超排行榜()
        {
            return @$"
  and [自營商買賣超] > 0
  and [自營商買賣超] * [Close] > 1000
  order by [自營商買賣超] *[Close] desc
";
        }

        private static string 自營賣超排行榜()
        {
            return @$"
  and [自營商買賣超] < 0
  and [自營商買賣超] * [Close] <- 1000
  order by [自營商買賣超] *[Close] asc
";
        }


        private static string 融資買超排行榜()
        {
            return @$"
  and ([融資買進] - [融資賣出])>0
  and ([融資買進] - [融資賣出]) * [Close] > 5000
  order by ([融資買進] - [融資賣出]) * [Close] desc 
";
        }

        private static string 融資賣超排行榜()
        {
            return @$"
  and ( [融資賣出] - [融資買進])>0
  and ( [融資賣出] - [融資買進]) * [Close] > 5000
  order by ([融資賣出] - [融資買進]) * [Close] desc 
";
        }

        private static string 融券買超排行榜()
        {
            return @$"
  and ([融券買進] - [融券賣出])>0
  order by ([融券買進] - [融券賣出]) * [Close] desc 
";
        }

        private static string 融券賣超排行榜()
        {
            return @$"
  and ( [融券賣出] - [融券買進])>0
  order by ( [融券賣出] - [融券買進]) * [Close] desc 
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
where StockId in (SELECT StockId FROM ranked_messages WHERE rn = 1 and Remark = '很好')
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

        #endregion
    }
}
