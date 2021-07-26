using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public enum BuyType
    {
        融資,
        外資,
        主力
    }


    public class SuddenlyInvestment : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;


        public SuddenlyInvestment(LineNotifyBotApi lineNotifyBotApi)
        { 
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public override async Task RunAsync()
        {
            Dictionary<BuyType, string> map = new Dictionary<BuyType, string>
            {
                { BuyType.主力,  "(a.主力買超張數 - a.主力賣超張數) > b.主力買賣超 * 2 and"},
                { BuyType.融資, "(a.融資買進 - a.融資賣出) > b.融資買賣超 * 2 and"},
                { BuyType.外資, "(a.外資買賣超) > b.外資買賣超 * 2 and"},
            };

            var context = new StockDbContext();
            //var dates = context.Prices.Where(p => p.StockId == "2330" && p.Datetime >= new DateTime(2020, 1, 1))
            //     .OrderByDescending(p => p.Datetime)
            //    .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
            //    .ToArray();

            //for (int i = 0; i < dates.Length; i++)
            {
                var date = DateTime.Today.ToString("yyyy-MM-dd");
                await ParseStocksAsync(context, date, map[BuyType.主力], BuyType.主力);
                await ParseStocksAsync(context, date, map[BuyType.融資], BuyType.融資);
                await ParseStocksAsync(context, date, map[BuyType.外資], BuyType.外資);
            }
        }

        private async Task ParseBrokersAsync(StockDbContext context, string bhid, string b, string date)
        {
            var sql = $"https://fubon-ebrokerdj.fbs.com.tw/z/zg/zgb/zgb0.djhtm?a={bhid}&b={b}&c=E&e={date}&f={date}";

        }

        private async Task ParseStocksAsync(StockDbContext context, string datetime, string filter, BuyType buyType)
        {
            var sql = $@"declare @datetime as Datetime = '{datetime}';

                with tmp as (
                select StockId, Name, 
	                avg(abs(融資買進-融資賣出)) as 融資買賣超,
	                avg(abs(主力買超張數-主力賣超張數)) as 主力買賣超,
	                avg(abs(外資買賣超)) as 外資買賣超
                from 
	                [Prices] p 
                  where  p.[Datetime] >= DATEADD(DD, -8, @datetime)  and p.[Datetime] <=DATEADD(DD, -1, @datetime)
                  group by StockId, Name)


                select a.* from [Prices] a  
                join tmp b on a.StockId = b.StockId
                where 
                a.[Datetime] = @datetime and
                {filter}
                a.VMA5 > 500
                order by a.漲跌百分比 desc
                ";


            var s = Stopwatch.StartNew();
            s.Start();
            var prices = await context.Prices.FromSqlRaw(sql).ToArrayAsync();

            for (int i = 0; i < prices.Length; i++)
            {
                var p = prices[i];
                if (p.Signal == null)
                {
                    p.Signal = $"{buyType}大買";
                }
                else
                {
                    p.Signal += $"::{buyType}大買";
                }

                Console.WriteLine($"{datetime} {p.StockId} {p.Name} {buyType}");
            }
            await context.SaveChangesAsync();

            s.Stop();
            Console.WriteLine($"TotalMinutes: {s.Elapsed.TotalMinutes}");
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
