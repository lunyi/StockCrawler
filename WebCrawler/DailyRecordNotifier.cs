using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class DailyRecordNotifier : BaseParser
    {
        public DailyRecordNotifier()
        { 
        }

        public override async Task RunAsync()
        {
            var context = new StockDbContext();

            var date = await context.Prices.Where(p => p.StockId == "2330" && p.Datetime <= new DateTime(2020, 11, 4) && p.Datetime >= new DateTime(2019,10,2))
                .OrderByDescending(p => p.Datetime)
                .Select(p => p.Datetime)
                .ToArrayAsync();

            for (int i = 0; i < date.Length; i++)
            {
                var 上漲破月線股票 = 上漲破月線(context, date[i], date[i+1]);
                var 盤整突破股票 = 盤整突破(context, date[i]);
                var 漲停板股票 = 漲停板(context, date[i]);

                Console.WriteLine($"{date[i]}:yyyy-MM-dd");
            }

            //await NotifyBotApiAsync(外資投信主力買超股票);
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

        private string 漲停板(StockDbContext context, DateTime date)
        {
            var prices = context.Prices.Where(p =>
               p.Datetime == date && p.漲跌百分比 > 9
           );

            var msg = new StringBuilder();
            msg.AppendLine($"漲停板股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var price in prices)
            {
                msg.AppendLine($"{index}. {price.StockId} {price.Name} {price.Close} {price.漲跌百分比}%");
                price.Signal = price.Signal == null ? "漲停板" : price.Signal += "::漲停板";
                index++;
            }

            context.SaveChanges();
            return msg.ToString();
        }

        private string 盤整突破(StockDbContext context, DateTime date)
        {
            var prices = context.Stocks.FromSqlRaw($"exec [usp_GetBreakThrough] '{date:yyyy-MM-dd}'").ToArray();

            var msg = new StringBuilder();
            msg.AppendLine($"盤整突破股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var price in prices)
            {
                msg.AppendLine($"{index}.[{price.Industry}] {price.StockId} {price.Name} {price.股價}");
                var p = context.Prices.FirstOrDefault(p => p.Datetime == date && p.StockId == price.StockId);

                if (p != null)
                    p.Signal = p.Signal == null ? "盤整突破" : p.Signal += "::盤整突破";
                index++;
            }
            context.SaveChanges();

            return msg.ToString();
        }

        private string 上漲破月線(StockDbContext context, DateTime date, DateTime preDate)
        {
            var sql = $@"select s.* from Stocks s join 
(select * from [Prices] a where a.Datetime = '{date:yyyy-MM-dd}') a1 on s.StockId = a1.StockId join 
(select * from [Prices] a where a.Datetime = '{preDate:yyyy-MM-dd}') a2 on a1.StockId = a2.StockId
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

            var stocks = context.Stocks.FromSqlRaw(sql).ToArray();

            var msg = new StringBuilder();
            msg.AppendLine($"上漲破月線股票 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var index = 1;
            foreach (var stock in stocks)
            {
                msg.AppendLine($"{index}.[{stock.Industry}]{stock.StockId} {stock.Name} {stock.股價}");

                var p = context.Prices.FirstOrDefault(p => p.Datetime == date && p.StockId == stock.StockId);
                p.Signal = p.Signal == null ? "破月線" : p.Signal += "::破月線";

                index++;
            }

            context.SaveChanges();
            return msg.ToString();

        }
    }
}
