using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Messages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebAutoCrawler
{
    public class ThousandDataCrawlerV2 : BaseCrawler
    {
        string BillionUrl = "https://www.tdcc.com.tw/smWeb/QryStock.jsp";

        public ThousandDataCrawlerV2() : base()
        {
            GoToUrl(BillionUrl);
        }

        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p => p.StockId).ToArray();

            foreach (var stock in stocks)
            {
                for (int k = 0; k < 13; k++)
                {
                    try
                    {
                        GoToUrl(BillionUrl);
                        Thread.Sleep(300);
                        var selectElement2 = new SelectElement(FindElement(By.Id("scaDates")));
                        var date = selectElement2.Options[k].Text.Insert(6,"-").Insert(4,"-");
                        selectElement2.SelectByIndex(k);
                        FindElement(By.Id("StockNo")).SendKeys(stock.StockId);
                        FindElement(By.Name("sub")).Click();

                        Console.WriteLine(stock.StockId + "--" + date);
                        Thread.Sleep(100);

                        var t = new Thousand();
                        t.Id = Guid.NewGuid();
                        t.StockId = stock.StockId;
                        t.Name = stock.Name;
                        t.Datetime = Convert.ToDateTime(date);
                        t.CreatedOn = DateTime.Now;

                        (t.P1, t.P20, t.P30) = GetPercent(2);
                        (t.P1, t.P20, t.P30) = GetPercent(3);
                        (t.P1, t.P20, t.P30) = GetPercent(4);
                        (t.P1, t.P20, t.P30) = GetPercent(5);
                        (t.P1, t.P20, t.P30) = GetPercent(6);
                        (t.P1, t.P20, t.P30) = GetPercent(7);
                        (t.P1, t.P20, t.P30) = GetPercent(8);
                        (t.P1, t.P20, t.P30) = GetPercent(9);
                        (t.P1, t.P20, t.P30) = GetPercent(10);
                        (t.P1, t.P20, t.P30) = GetPercent(11);
                        (t.P1, t.P20, t.P30) = GetPercent(12);
                        (t.P1, t.P20, t.P30) = GetPercent(13);
                        (t.P1, t.P20, t.P30) = GetPercent(14);
                        (t.P1, t.P20, t.P30) = GetPercent(15);
                        (t.P1, t.P20, t.P30) = GetPercent(16);

                        //context.Thousand.Add(t);
                        //await context.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed {ex}!");
                    }
                }
            }
        }

        private (int, decimal, decimal) GetPercent(int index)
        {
            var p1 = Convert.ToInt32(FindElement(By.XPath(@$"//*[@id='Qform']/table/tbody/tr/td/table[6]/tbody/tr[{index}]/td[3]")).Text.Replace(",",""));
            var s1 = Convert.ToDecimal(FindElement(By.XPath(@$"//*[@id='Qform']/table/tbody/tr/td/table[6]/tbody/tr[{index}]/td[4]")).Text.Replace(",", ""))/1000;
            var c1 = Convert.ToDecimal(FindElement(By.XPath(@$"//*[@id='Qform']/table/tbody/tr/td/table[6]/tbody/tr[{index}]/td[5]")).Text.Replace(",", ""));
            return (p1, s1, c1);
        }
    }
}
