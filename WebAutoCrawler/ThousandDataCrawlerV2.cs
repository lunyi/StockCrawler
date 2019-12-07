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
                for (int k = 15; k < 50; k++)
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

                        var p = context.Thousand.FirstOrDefault(p => p.StockId == stock.StockId && p.Datetime == t.Datetime);
                        if (p != null)
                        {
                            continue;
                        }

                        decimal temp = 0;
                        (t.C1, t.S1, t.P1) = GetPercent(2);
                        (t.C5, t.S5, t.P5) = GetPercent(3);
                        (t.C10, t.S10, t.P10) = GetPercent(4);
                        (t.C15, t.S15, t.P15) = GetPercent(5);
                        (t.C20, t.S20, t.P20) = GetPercent(6);
                        (t.C30, t.S30, t.P30) = GetPercent(7);
                        (t.C40, t.S40, t.P40) = GetPercent(8);
                        (t.C50, t.S50, t.P50) = GetPercent(9);
                        (t.C100, t.S100, t.P100) = GetPercent(10);
                        (t.C200, t.S200, t.P200) = GetPercent(11);
                        (t.C400, t.S400, t.P400) = GetPercent(12);
                        (t.C600, t.S600, t.P600) = GetPercent(13);
                        (t.C800, t.S800, t.P800) = GetPercent(14);
                        (t.C1000, t.S1000, t.P1000) = GetPercent(15);
                        (t.COver1000, t.SOver1000, t.POver1000) = GetPercent(16);
                        (t.TotalCount, t.TotalStock, temp) = GetPercent(17);
                        t.PUnder100 = t.P1 + t.P5 + t.P10 + t.P15 + t.P20 + t.P30 + t.P40 + t.P50 + t.P100;
                        t.CUnder100 = t.C1 + t.C5 + t.C10 + t.C15 + t.C20 + t.C30 + t.C40 + t.C50 + t.C100;
                        t.SUnder100 = t.S1 + t.S5 + t.S10 + t.S15 + t.S20 + t.S30 + t.S40 + t.S50 + t.S100;

                        context.Thousand.Add(t);
                        await context.SaveChangesAsync();

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
