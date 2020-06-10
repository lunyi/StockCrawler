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

        public HistoryPriceCrawler() : base()
        {

        }
        public override Task ExecuteAsync()
        {
            return Task.FromResult(true);
        }

        private void TryCloseAdIfExists()
        {
            try
            {
                var cancelBtn = FindElement(By.ClassName("img_cancel"));

                if (cancelBtn != null)
                {
                    cancelBtn.Click();
                }
            }
            catch (NoSuchElementException)
            { 
                
            }
            finally
            {

            }
        }
        public List<HistoryPrice> Execute(string stockId)
        {
            var url = $"https://www.cnyes.com/twstock/ps_historyprice/{stockId}.htm";

            GoToUrl(url);
            TryCloseAdIfExists();

            var input = FindElement(By.Name("ctl00$ContentPlaceHolder1$startText"));
            for (int i = 0; i < 10; i++)
            {
                input.SendKeys(Keys.Backspace);
            }

            FindElement(By.XPath("//*[@id='datepicker_div']/div[3]/div/select[2]/option[@value='2017']")).Click();
            FindElement(By.XPath("//*[@id='datepicker_div']/div[3]/div/select[1]/option[@value='0']")).Click();
            FindElement(By.XPath("//*[@id='datepicker_div']/div[3]/table/tbody/tr[1]/td[4]/a")).Click();
            FindElement(By.Name("ctl00$ContentPlaceHolder1$submitBut")).Click();

            Thread.Sleep(1000);

            var tabs = FindElements(By.XPath("//*[@id='main3']/div[5]/div[3]/table/tbody/tr"));

            var prices = new List<HistoryPrice>();
            for (int i = 1; i < tabs.Count; i++)
            {
                var price = new HistoryPrice();
                price.Id = Guid.NewGuid();
                price.CreatedOn = DateTime.Now;

                var td = tabs[i].FindElements(By.TagName("td"));
                price.Datetime = Convert.ToDateTime(td[0].Text);
                price.Open = Convert.ToDecimal(td[1].Text);
                price.High = Convert.ToDecimal(td[2].Text);
                price.Low = Convert.ToDecimal(td[3].Text);
                price.Close = Convert.ToDecimal(td[4].Text);
                price.漲跌 = Convert.ToDecimal(td[5].Text);
                price.漲跌百分比 = Convert.ToDecimal(td[6].Text.Replace("%",""));
                price.成交量 = Convert.ToInt32(td[7].Text.Replace(",", ""));
                price.成交金額 = Convert.ToInt32(td[8].Text.Replace(",", ""));
                price.本益比 = Convert.ToDecimal(td[9].Text);
                prices.Add(price);
            }
            return prices.OrderByDescending(p => p.Datetime).ToList();
        }
    }
}