using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebAutoCrawler
{
    public class DailyTraderCrawler : BaseCrawler
    {
        string HealthCheckUrl = "https://goodinfo.tw/StockInfo/DayTrading.asp?STOCK_ID={0}";

        public DailyTraderCrawler() : base()
        {
        }
        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToList();

            foreach (var stock in stocks)
            {
                try
                {
                    var season = Parser(string.Format(HealthCheckUrl, stock.StockId), stock.StockId, stock.Name);
                    context.SeasonData.AddRange(season);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} {ex} !");
                }
            }
        }

        private SeasonData[] Parser(string url, string stockId, string name)
        {
            GoToUrl(url);

            var selectElement = new SelectElement(FindElement(By.Id("selDayTradingPeriod")));
            selectElement.SelectByIndex(1);


            var test = FindElement(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[1]"));
            var season   = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[1]/th"));

            var seasonData = new List<SeasonData>();
            for (var i = 1; i < season.Count; i++)
            {
                var q = Convert.ToInt32(season[i].Text.Substring(5, 1)) * 3;
                var datetime = Convert.ToDateTime(season[i].Text.Substring(0, 4) + "-" + q.ToString("00") + "-01"); 
                seasonData.Add(new SeasonData
                {
                    Id = Guid.NewGuid(),
                    StockId = stockId,
                    Name = name,
                    CreatedOn = DateTime.Now,
                    Datetime = datetime,
                });          
            }
            return seasonData.OrderByDescending(p=>p.Datetime).ToArray();
        }

    
    }
}
