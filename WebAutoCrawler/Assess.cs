using DataService.Models;
using Messages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAutoCrawler
{
    public class Assess
    {
        IWebDriver _driver;
        string HealthCheckUrl = "https://www.cmoney.tw/finance/f00025.aspx?s={0}";
        string[] NotContainStocks = new string[] { 
                "0050",
                "0051",
                "0052",
                "0053",
                "0054",
                "0055",
                "0056",
                "0057",
                "0058",
                "0059",
                "0060",
                "0061"
            };
        public Assess()
        {
            ChromeOptions chromeBrowserOptions = new ChromeOptions();
            //需要阻擋跳出視窗時，可將下面註解移除
            chromeBrowserOptions.AddArgument("--disable-popup-blocking");
            //要節省流量，不載入圖片的時候，可將下面註解移除
            //chromeBrowserOptions.AddExtension(@"ChromeDriver\Block-image_v1.1.crx"); // 載入阻擋圖片外掛程式
            _driver = new ChromeDriver(@"..\..\..\..\BrowserPath", chromeBrowserOptions);
            //可避免網頁被cache住，一直查到舊資料(需不斷重複查詢同一網頁時就會用到)
            _driver.Manage().Cookies.DeleteAllCookies();
        }
        public async Task Execute()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks
                .Where(p => !NotContainStocks.Contains(p.StockId))
                .OrderBy(p => p.StockId)
                .ToList();

            foreach (var stock in stocks)
            {
                try
                {
                    _driver.Navigate().GoToUrl(string.Format(HealthCheckUrl, stock.StockId));

                    Thread.Sleep(200);
                    var checks = _driver.FindElement(By.ClassName("remark"));
                    var barnums = _driver.FindElements(By.ClassName("bar-num2"));
                    var s = barnums[0].Text;

                    var item = new Remarks
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.StockId,
                        Name = stock.Name,
                        Remark = checks.Text,
                        價值 = Convert.ToInt32(barnums[0].Text),
                        安全 = Convert.ToInt32(barnums[1].Text),
                        成長 = Convert.ToInt32(barnums[2].Text),
                        籌碼 = Convert.ToInt32(barnums[3].Text),
                        技術 = Convert.ToInt32(barnums[4].Text),
                        CreatedOn = DateTime.Now,
                    };
                    context.Remarks.Add(item);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
                }
            }
        }

        public void Complete()
        {
            _driver.Close();
            _driver.Dispose();
        }
    }
}
