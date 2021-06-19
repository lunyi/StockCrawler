using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using OpenQA.Selenium;

namespace WebAutoCrawler
{
    public class FutuneEngineCrawler : BaseCrawler
    {
        string HealthCheckUrl = "http://www.fortunengine.com.tw/evaluator.aspx?menu=on&scode={0}";
        public FutuneEngineCrawler() : base()
        {

        }

        public AnaFutureEngine[] GetAnaFutureEngines(string stockId, string stockName)
        {
            GoToUrl(string.Format(HealthCheckUrl, stockId));

            Thread.Sleep(200);

            var list = new List<AnaFutureEngine>();
            for (int i = 1; i <= 5; i++)
            {
                var title1 = FindElement(By.Id($"tab_{i}"));
                title1.Click();
                Thread.Sleep(200);

                var desc = FindElement(By.Id("tbIndcat"));
                var ths = FindElements(By.ClassName("th"));
                var fas = FindElements(By.ClassName("fas"));

                for (int j = 0; j < ths.Count; j++)
                {
                    var foo = fas[j].GetAttribute("class");
                    var ana = new AnaFutureEngine
                    {
                        Id = Guid.NewGuid(),
                        StockId = stockId,
                        Name = stockName,
                        Description = ths[j].Text,
                        Pass = foo != "fas fa-times",
                        Type = title1.Text,
                        CreatedOn = DateTime.Now,
                    };

                    list.Add(ana);
                }
            }

            return list.ToArray();
        }
        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            //var s = context.Stocks.FromSqlRaw(GetSql()).ToList();
            var stocks = context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToList();
            var failCount = 1;
            foreach (var stock in stocks)
            {
                try
                {
                    GoToUrl(string.Format(HealthCheckUrl, stock.StockId));

                    var list = GetAnaFutureEngines(stock.StockId, stock.Name);
                    Thread.Sleep(200);
                    //await context.AnaFutureEngines.AddRangeAsync(list);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    Console.WriteLine($"{failCount} : {stock.StockId} {stock.Name} Parser Failed !");
                    failCount++;
                }
            }
        }


        string GetSql()
        {
            return @"
            SELECT a.*
  FROM [dbo].[Stocks] a 
  left join [dbo].[Remarks] b on a.StockID = b.StockId
  where b.StockId is null
            ";
        }
    }
}
