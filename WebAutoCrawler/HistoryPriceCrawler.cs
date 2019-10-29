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
    public class HistoryPriceCrawler : BaseCrawler
    {
        string Url = "https://www.cnyes.com/twstock/ps_historyprice/2330.htm";

        public HistoryPriceCrawler() : base()
        {

        }
        //public override async Task ExecuteAsync()
        //{
        //    var context = new StockDbContext();
        //    var stocks = context.Stocks
        //        .Where(p => p.Status == 1)
        //        .OrderBy(p => p.StockId)
        //        .ToList();

        //    foreach (var stock in stocks)
        //    {
        //        try
        //        {
        //            var checks = Parser(string.Format(HealthCheckUrl, stock.StockId));
        //            foreach (var check in checks)
        //            {
        //                foreach (var checkItem in check.Value)
        //                {
        //                    var item = new AnaStatementDogs
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        StockId = stock.StockId,
        //                        Name = stock.Name,
        //                        Type = check.Key,
        //                        Pass = checkItem.Result,
        //                        Description = checkItem.Name,
        //                        CreatedOn = DateTime.Now,
        //                    };
        //                    context.AnaStatementDogs.Add(item);
        //                }
        //            }
        //            await context.SaveChangesAsync();
        //        }
        //        catch (Exception)
        //        {
        //            Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
        //        }
        //    }
        //}

        public override async Task ExecuteAsync()
        {
            GoToUrl(Url);

            var cancelBtn = FindElement(By.ClassName("img_cancel"));

            if (cancelBtn != null)
            {
                cancelBtn.Click();
            }

            var input = FindElement(By.Name("ctl00$ContentPlaceHolder1$startText"));
            for (int i = 0; i < 10; i++)
            {
                input.SendKeys(Keys.Backspace);
            }

            FindElement(By.XPath("//*[@id='datepicker_div']/div[3]/div/select[2]/option[@value='2014']")).Click();
            FindElement(By.XPath("//*[@id='datepicker_div']/div[3]/div/select[1]/option[@value='0']")).Click();
            FindElement(By.XPath("//*[@id='datepicker_div']/div[3]/table/tbody/tr[1]/td[4]/a")).Click();
            FindElement(By.Name("ctl00$ContentPlaceHolder1$submitBut")).Click();

            Thread.Sleep(1000);

            var tabs = FindElements(By.XPath("//*[@id='main3']/div[5]/div[3]/table/tbody/tr"));

            for (int i = 1; i < tabs.Count; i++)
            {
                var td = tabs[i].FindElements(By.TagName("td"));
                var datetime = td[0].Text;
            }

        }
    }
}