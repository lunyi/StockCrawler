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
    public class HealthCheck
    {
        IWebDriver _driver;
        string LoginUrl = "https://statementdog.com/users/sign_in";
        string HealthCheckUrl = "https://statementdog.com/analysis/tpe/{0}/stock-health-check";
        string Username = "lunyi.lester@gmail.com";
        string Password = "1q2w3e4r";
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
        public HealthCheck()
        {
            ChromeOptions chromeBrowserOptions = new ChromeOptions();
            //需要阻擋跳出視窗時，可將下面註解移除
            chromeBrowserOptions.AddArgument("--disable-popup-blocking");
            //要節省流量，不載入圖片的時候，可將下面註解移除
            //chromeBrowserOptions.AddExtension(@"ChromeDriver\Block-image_v1.1.crx"); // 載入阻擋圖片外掛程式
            _driver = new ChromeDriver(@"..\..\..\..\BrowserPath", chromeBrowserOptions);
            //可避免網頁被cache住，一直查到舊資料(需不斷重複查詢同一網頁時就會用到)
            _driver.Manage().Cookies.DeleteAllCookies();
            _driver.Navigate().GoToUrl(LoginUrl);
            var emailElement = _driver.FindElement(By.Id("user_email"));
            var passwordElement = _driver.FindElement(By.Id("user_password"));
            var submitElement = _driver.FindElement(By.ClassName("submit-btn"));

            emailElement.SendKeys(Username);
            passwordElement.SendKeys(Password);
            submitElement.Click();
        }
        public async Task SetHealthCheckAsync()
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
                    var checks = Parser(string.Format(HealthCheckUrl, stock.StockId));
                    foreach (var check in checks)
                    {
                        foreach (var checkItem in check.Value)
                        {
                            var item = new Checks
                            {
                                Id = Guid.NewGuid(),
                                StockId = stock.StockId,
                                Name = stock.Name,
                                Type = check.Key,
                                Pass = checkItem.Result,
                                Description = checkItem.Name,
                                CreatedOn = DateTime.Now,
                            };
                            context.Checks.Add(item);
                        }
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
                }
            }
        }

        private Dictionary<string, CheckModel[]> Parser(string url)
        {
            _driver.Navigate().GoToUrl(url);

            var checks = _driver.FindElements(By.ClassName("stock-health-check-module"));
            var list = new Dictionary<string, CheckModel[]>();

            for (int i = 0; i < 4; i++)
            {
                var check = checks[i];
                var titleItem = check.FindElement(By.ClassName("stock-health-check-module-title"));
                var item = check.FindElement(By.ClassName("stock-health-check-module-modal-active-btn"));
                Thread.Sleep(500);
                item.Click();
                var checkItems = check.FindElements(By.ClassName("stock-health-check-item"));
                var checkModels = new List<CheckModel>();

                foreach (var checkItem in checkItems)
                {
                    var result = checkItem.FindElement(By.ClassName("stock-health-check-item-result"));
                    var name = checkItem.FindElement(By.ClassName("stock-health-check-item-name"));

                    var model = new CheckModel
                    {
                        Name = name.Text,
                        Result = result.Text == "通過" ? true : false
                    };
                    checkModels.Add(model);
                }

                list.Add(titleItem.Text, checkModels.ToArray());
                var closeBtn = check.FindElement(By.ClassName("modal-btn"));
                closeBtn.Click();
            }
            return list;
        }

        public void Complete()
        {
            _driver.Close();
            _driver.Dispose();
        }
    }
}
