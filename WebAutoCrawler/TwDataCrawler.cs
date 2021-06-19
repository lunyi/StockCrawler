using DataService.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAutoCrawler
{
    public class TwDataCrawler : BaseCrawler
    {
        string BillionUrl = "https://histock.tw/stock/indicator.aspx";

        public TwDataCrawler() : base()
        {
        }

        public async Task ExecuteLatestAsync()
        {
            GoToUrl(string.Format(BillionUrl));

            var selectElement = new SelectElement(FindElement(By.XPath($@"//*[@id='CPHB1_ctl00_ddlDate']")));
            var res = selectElement.Options[0].Text.Insert(6, "-").Insert(4, "-");



            try
            {
                   

                    
            }
            catch (Exception)
            {
                Console.WriteLine($"TwDataCrawler Parser Failed !");
            }
        }

        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p => p.StockId).ToList();

            foreach (var stock in stocks)
            {
                try
                {
                    GoToUrl(string.Format(BillionUrl, stock.StockId));

                    Thread.Sleep(400);
                    var overFourHundreds = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[2]/table/tbody/tr[3]/td"));
                    var underFourHundreds = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[2]/table/tbody/tr[2]/td"));
                    var titles = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[2]/table/tbody/tr[1]/th"));
                    var p1 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[2]/td"));
                    var p5 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[3]/td"));
                    var p10 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[4]/td"));
                    var p15 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[5]/td"));
                    var p20 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[6]/td"));
                    var p30 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[7]/td"));
                    var p40 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[8]/td"));
                    var p50 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[9]/td"));
                    var p100 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[10]/td"));
                    var p200 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[11]/td"));
                    var p400 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[12]/td"));
                    var p600 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[13]/td"));
                    var p800 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[14]/td"));
                    var p1000 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[15]/td"));
                    var overThousand = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div[1]/table/tbody/tr[16]/td"));

                    for (int i = 1; i < titles.Count; i++)
                    {
                        var s = new Thousand
                        {
                            StockId = stock.StockId,
                            Name = stock.Name,
                            Datetime = Convert.ToDateTime(titles[i].Text + "/01"),
                            //PercentOver400 = Convert.ToDecimal(overFourHundreds[i].Text),
                            //PercentUnder400 = Convert.ToDecimal(underFourHundreds[i].Text),
                            P1 = Convert.ToDecimal(p1[i].Text),
                            P5 = Convert.ToDecimal(p5[i].Text),
                            P10 = Convert.ToDecimal(p10[i].Text),
                            P15 = Convert.ToDecimal(p15[i].Text),
                            P20 = Convert.ToDecimal(p20[i].Text),
                            P30 = Convert.ToDecimal(p30[i].Text),
                            P40 = Convert.ToDecimal(p40[i].Text),
                            P50 = Convert.ToDecimal(p50[i].Text),
                            P100 = Convert.ToDecimal(p100[i].Text),
                            P200 = Convert.ToDecimal(p200[i].Text),
                            P400 = Convert.ToDecimal(p400[i].Text),
                            P600 = Convert.ToDecimal(p600[i].Text),
                            P800 = Convert.ToDecimal(p800[i].Text),
                            P1000 = Convert.ToDecimal(p1000[i].Text),
                            //PercentOver1000 = Convert.ToDecimal(overThousand[i].Text),
                            CreatedOn = DateTime.Now
                        };

                        context.Thousands.Add(s);
                    }

                    await context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
                }
            }
        }
    }
}
