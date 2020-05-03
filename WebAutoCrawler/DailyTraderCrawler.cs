using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.DOM;
using OpenQA.Selenium.Support.UI;

namespace WebAutoCrawler
{
    public class DailyTraderCrawler : BaseCrawler
    {
        private string allStockUrl = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E7%8F%BE%E8%82%A1%E7%95%B6%E6%B2%96%E7%8E%87+%28%E7%95%B6%E6%97%A5%29%40%40%E7%8F%BE%E8%82%A1%E7%95%B6%E6%B2%96%E7%8E%87%40%40%E7%95%B6%E6%97%A5";
        string HealthCheckUrl = "https://goodinfo.tw/StockInfo/DayTrading.asp?STOCK_ID={0}";

        public DailyTraderCrawler() : base()
        {
        }
        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks
                .Where(p => p.Status == 1)
                .OrderByDescending(p => p.StockId)
                .ToList();

            //var stock = stocks.FirstOrDefault(p=>p.StockId == "9940");
            foreach (var stock in stocks)
            {
                try
                {
                    await ParserAsync(context, string.Format(HealthCheckUrl, stock.StockId), stock.StockId, stock.Name);
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                    Console.WriteLine($"{stock.StockId} {stock.Name} {ex} !");
                }
            }
        }

        private async Task ParserAsync(StockDbContext context, string url, string stockId, string name)
        {
            GoToUrl(url);

            var selectElement = new SelectElement(FindElement(By.Id("selDayTradingPeriod")));
            selectElement.SelectByIndex(1);

            for (var i = 1; i <= 7; i++)
            {
                var tables = FindElement(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div/div/div/table/tbody[{i}]"));

                if (tables == null)
                {
                    continue;
                }

                var tr = tables.FindElements(By.TagName("tr"));

                foreach (var t in tr)
                {
                    var td = t.FindElements(By.TagName("td"));

                    var year = Convert.ToInt16(td[0].Text.Substring(0, 2)) >= 10 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                    var date = Convert.ToDateTime($"{year}/{td[0].Text}");

                    var price = context.Prices.FirstOrDefault(p => p.StockId == stockId && p.Datetime == date);

                    if (price == null) continue;

                    price.當沖張數 = Convert.ToDecimal(td[6].Text);
                    price.當沖比例 = Convert.ToDecimal(td[7].Text);
                    price.當沖總損益 = Convert.ToDecimal(td[12].Text);
                    price.當沖均損益 = Convert.ToDecimal(td[13].Text);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
