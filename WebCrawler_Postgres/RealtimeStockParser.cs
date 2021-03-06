﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgresData.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class RealtimeStockParser : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;

        public RealtimeStockParser(LineNotifyBotApi lineNotifyBotApi)
        { 
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public override async Task RunAsync()
        {
            var context = new stockContext();

            await ParseStockAsync();

            _token = await context.Tokens.Select(p => p.LineToken).FirstOrDefaultAsync();
            //var 外資投信主力買超股票 = Get外資投信主力買超股票(context);

            var 上漲破月線股票 = 上漲破月線(context);
            var 盤整突破股票 = 盤整突破(context);

            var 上漲類股1 = 上漲類股(context);
            await NotifyBotApiAsync(上漲類股1);

            await NotifyBotApiAsync(上漲破月線股票);
            await NotifyBotApiAsync(盤整突破股票);
        }


        private string 上漲類股(stockContext context)
        {
            var sql = $@"select a.*, b.totalCount,  100 * CAST(a._count AS DECIMAL(18,2))  / b.totalCount as [percent] from (
select  s.Industry, count(1) as _count from [Price] p 
join [Stock] s on p.StockId = s.StockId 
where p.[Datetime] = '{DateTime.Now:yyyy-MM-dd}'　and  p.漲跌百分比 >=4
group by  s.Industry) a
join (select z.Industry, count(1) as totalCount from Stock z　group by  ｚ.Industry) b on b.Industry = a.Industry
where a._count >=10
order by　a._count　desc, CAST(a._count AS DECIMAL(18,2))  / b.totalCount desc
";

            var stocks = context._Industries.FromSqlRaw(sql).ToArray();

            var msg = new StringBuilder();
            msg.AppendLine($"上漲類股 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.Industry} {stock._count}/{stock.totalCount}");
                index++;
            }

            context.SaveChanges();
            return msg.ToString();

        }

        private async Task ParseStockAsync()
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var context = new stockContext();
            var today = DateTime.Today;
            var stocks = await context.Stocks.Where(p => p.Status == 1)
               .ToArrayAsync();

            var Price = await context.Prices.Where(p => p.Datetime == today)
                .ToArrayAsync();
            var count = 0;
            for (int i = 1; i <= 41; i++)
            {
                var url = $@"https://histock.tw/stock/rank.aspx?&p={i}&d=1";
                var rootNode = GetRootNoteByUrl(url);
                var nodes = rootNode.SelectNodes("//html/body/form/div[4]/div[5]/div/div/div/div[2]/div[2]/table/tr");

                for (int j = 1; j < nodes.Count; j++)
                {
                    var tds = nodes[j].SelectNodes("td");

                    var checkStock = stocks.FirstOrDefault(p => p.StockId == tds[0].InnerText);
                    if (checkStock == null)
                        continue;
                    count++;
                    var exists = true;
                    var p = Price.FirstOrDefault(p => p.StockId == tds[0].InnerText);
                    if (p == null)
                    {
                        exists = false;
                        p = new Price();
                    }

                    p.StockId = tds[0].InnerText;
                    p.Name = tds[1].SelectSingleNode("a").InnerText;
                    p.Datetime = today;
                    p.Close = Convert.ToDecimal(tds[2].SelectSingleNode("span").InnerText);
                    var 漲跌 = tds[3].SelectSingleNode("span").InnerText;
                    p.漲跌 = 漲跌 == "--" ? 0 : Convert.ToDecimal(漲跌.Replace("▼", "").Replace("▲", ""));
                    p.漲跌百分比 = 漲跌 == "--" ? 0 : Convert.ToDecimal(tds[4].SelectSingleNode("span").InnerText.Replace("-", "").Replace("+", "").Replace("%", ""));
                    p.Open = Convert.ToDecimal(tds[7].InnerText);
                    p.High = Convert.ToDecimal(tds[8].SelectSingleNode("span").InnerText);
                    p.Low = Convert.ToDecimal(tds[9].SelectSingleNode("span").InnerText);
                    p.成交量 = Convert.ToInt32(tds[11].InnerText.Replace(",", ""));
                    p.CreatedOn = DateTime.Now;
                    context.Entry(p).State = exists ? EntityState.Modified : EntityState.Added;
                    await context.SaveChangesAsync();
                    Console.WriteLine($"{i} {count} {p.StockId} {p.Name} {s.Elapsed.TotalSeconds}");
                }
            }

            s.Stop();
            Console.WriteLine($"TotalMinutes: {s.Elapsed.TotalMinutes}");
        }

        private string 盤整突破(stockContext context)
        {
            var Price = context.Stocks.FromSqlRaw($"exec [usp_GetRealtimeBreakThrough]'{DateTime.Now:yyyy-MM-dd}'").ToArray();

            var msg = new StringBuilder();
            msg.AppendLine($"當天盤整突破股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var price in Price)
            {
                msg.AppendLine($"{index}. {price.StockId} {price.Name} {price.股價} [{price.Industry}]({price.Description})");
                var p = context.Prices.FirstOrDefault(p => p.Datetime == DateTime.Today && p.StockId == price.StockId);
                p.Signal = (p.Signal == null || p.Signal.Contains("當天盤整突破")) ? "當天盤整突破" : p.Signal += "::當天盤整突破";
                index++;
            }
            context.SaveChanges();

            return msg.ToString();
        }

        private string 上漲破月線(stockContext context)
        {
            var datetime2 = context.Prices.Where(p => p.StockId == "2330" && p.Datetime < DateTime.Today)
                .OrderByDescending(p => p.Datetime)
                .Take(1)
                .Select(p => p.Datetime)
                .FirstOrDefault().ToString("yyyy-MM-dd");

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
      ,CAST((a1.[成交量]) AS nvarchar(30)) as [Description]
      ,s.股票期貨
from Stock s join 
(select * from [Price] a where a.Datetime = '{DateTime.Today:yyyy-MM-dd}') a1 on s.StockId = a1.StockId join 
(select * from [Price] a where a.Datetime = '{datetime2}') a2 on a1.StockId = a2.StockId
where a1.[Close] > a2.MA20 
	and a1.[Close] > a2.MA60 
	and a2.[Close] < a2.MA20 
	and a1.漲跌百分比 > 3 
	and a1.成交量 > a2.VMA5 * 2
	and a1.成交量 > 0
	and a1.[Close] > a1.[Open]
order by a1.成交量 desc
";

            var stocks = context.Stocks.FromSqlRaw(sql).ToArray();

            var msg = new StringBuilder();
            msg.AppendLine($"當天上漲破月線股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}. {stock.StockId} {stock.Name} {stock.股價} [{stock.Industry}]({stock.Description}) ");

                var p = context.Prices.FirstOrDefault(p => p.Datetime == DateTime.Today && p.StockId == stock.StockId);
                p.Signal = (p.Signal == null || p.Signal.Contains("當天破月線")) ? "當天破月線" : p.Signal += "::當天破月線";

                index++;
            }

            context.SaveChanges();
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
