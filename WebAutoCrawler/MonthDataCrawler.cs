using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Messages;
using OpenQA.Selenium;

namespace WebAutoCrawler
{
    public class MonthDataCrawler : BaseCrawler
    {
        string LoginUrl = "https://statementdog.com/users/sign_in";
        string Username = "lunyi.lester@gmail.com";
        string Password = "1q2w3e4r";
        string HealthCheckUrl = "https://histock.tw/stock/financial.aspx?no={0}";

        public MonthDataCrawler() : base()
        {
            //GoToUrl(LoginUrl);
            //var emailElement = FindElement(By.Id("user_email"));
            //var passwordElement = FindElement(By.Id("user_password"));
            //var submitElement = FindElement(By.ClassName("submit-btn"));

            //emailElement.SendKeys(Username);
            //passwordElement.SendKeys(Password);
            //submitElement.Click();
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
                    var monthData = Parser(string.Format(HealthCheckUrl, stock.StockId), stock.StockId, stock.Name);
                    context.MonthData.AddRange(monthData);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} {ex} !");
                }
            }
        }

        private MonthData[] Parser(string url, string stockId, string name)
        {
            GoToUrl(url);

            var table = FindElement(By.XPath("//*[@id='form1']/div[4]/div[3]/div[2]/div[1]/div[1]/div/div[5]/div/table"));

            var rows = table.FindElements(By.TagName("tr"));

            var monthData = new List<MonthData>();
            for (int i = 2; i < rows.Count; i++)
            {
                var tds = rows[i].FindElements(By.TagName("td"));

                monthData.Add(new MonthData
                {
                    Id = Guid.NewGuid(),
                    StockId = stockId,
                    Name = name,
                    CreatedOn = DateTime.Now,
                    Datetime = Convert.ToDateTime(tds[0].Text + "/01"),
                    單月營收 = Convert.ToDecimal(tds[1].Text.Replace(",","")),
                    去年同月營收 = Convert.ToDecimal(tds[2].Text.Replace(",", "")),
                    單月月增率 = Convert.ToDecimal(tds[3].Text.Replace("%", "")),
                    單月年增率 = Convert.ToDecimal(tds[4].Text.Replace("%", "")),
                    累計營收 = Convert.ToDecimal(tds[5].Text.Replace(",", "")),
                    去年累計營收 = Convert.ToDecimal(tds[6].Text.Replace(",", "")),
                    累積年增率 = Convert.ToDecimal(tds[7].Text.Replace("%", "")),
                });          
            }

            return monthData.ToArray();
        }
    }
}
