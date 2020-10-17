using DataService.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAutoCrawler
{
    public class DirectorSupervisorCrawler : BaseCrawler
    {
        public override async Task ExecuteAsync()
        {
            string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E5%85%A8%E9%AB%94%E8%91%A3%E7%9B%A3%E6%8C%81%E8%82%A1%E6%AF%94%E4%BE%8B%28%25%29%40%40%E5%85%A8%E9%AB%94%E8%91%A3%E7%9B%A3%40%40%E6%8C%81%E8%82%A1%E6%AF%94%E4%BE%8B%28%25%29";
            GoToUrl(url);
            Thread.Sleep(5000);

            //for (int i = 0; i <= 5; i++)
            //{
            //    var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
            //    selRANK.SelectByIndex(i);
            //    Thread.Sleep(10000);

            //    var tables = FindElements(By.XPath($"/html/body/table[5]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
            //    foreach (var table in tables)
            //    {
            //        var tr = table.FindElements(By.TagName("tr"));

            //        foreach (var t in tr)
            //        {
            //            try
            //            {
            //                var td = t.FindElements(By.TagName("td"));
            //                if (td.Count >= 10 && td[10].Text.Trim() == "-")
            //                    continue;
            //                if (td.Count <= 3)
            //                    continue;

            //                var year = DateTime.Now.Year;

            //                var datetime = Convert.ToDateTime($"{year}/{td[10].Text}");
            //                var stockId = Convert.ToString(td[1].Text);
            //                var updatedPrice = prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

            //                if (updatedPrice == null)
            //                    continue;

            //                Console.WriteLine($"DailyTrader {td[10].Text} {td[0].Text} {td[1].Text} {td[2].Text}");

            //                var name = Convert.ToString(td[2].Text);
            //                updatedPrice.當沖張數 = Convert.ToInt32(td[11].Text.Replace(",", ""));
            //                updatedPrice.當沖比例 = Convert.ToDecimal(td[12].Text);
            //                updatedPrice.當沖總損益 = Convert.ToDecimal(td[17].Text);
            //                updatedPrice.當沖均損益 = td[18].Text == "" ? 0 : Convert.ToDecimal(td[18].Text);
            //                updatedPrices.Add(updatedPrice);

            //                monthData.Close = Convert.ToDecimal(tds[1].Text.Replace(",", ""));
            //                monthData.Percent = Convert.ToDecimal(tds[3].Text);
            //                monthData.董監持股比例 = Convert.ToDecimal(tds[16].Text);
            //                monthData.董監持股增減 = Convert.ToDecimal(tds[17].Text);
            //                Console.WriteLine($"{stocks[i].StockId} {tds[0].Text} Updated");

            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex);
            //            }
            //        }
            //    }
            //}

            //return updatedPrices;
        }
        public async Task ExecuteHistoryAsync()
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var context = new StockDbContext();

            //var stocks = context.Stocks.Where(p => p.Status == 1).ToArray();
            var stocks = context.Stocks.FromSqlRaw(GetStockIdbyString()).ToArray();

            Thread.Sleep(2000);

            for (int i = 0; i <= stocks.Length; i++)
            {
                var url = $"https://goodinfo.tw/StockInfo/StockDirectorSharehold.asp?STOCK_ID={stocks[i].StockId}";
                GoToUrl(url);
                Thread.Sleep(3000);

                Console.WriteLine($"{stocks[i].StockId}");

                var tables = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div/div/table/tbody"));

                for (int ii = 0; ii < tables.Count; ii++)
                {
                    var trs = tables[ii].FindElements(By.TagName("tr"));
                    //var trs = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div/div/table/tbody[1]/tr"));

                    try
                    {
                        for (int j = 1; j < trs.Count; j++)
                        {
                            var tds = trs[j].FindElements(By.XPath("td"));
                            var date = Convert.ToDateTime(tds[0].Text + "/01");
                            var monthData = context.MonthData.FirstOrDefault(p => p.StockId == stocks[i].StockId && p.Datetime == date);

                            if (monthData != null)
                            {
                                if (tds[16].Text == "-")
                                {
                                    continue;
                                }
                                monthData.Close = Convert.ToDecimal(tds[1].Text.Replace(",", ""));
                                monthData.Percent = Convert.ToDecimal(tds[3].Text);
                                monthData.董監持股比例 = Convert.ToDecimal(tds[16].Text);
                                monthData.董監持股增減 = Convert.ToDecimal(tds[17].Text);
                                Console.WriteLine($"{stocks[i].StockId} {tds[0].Text} Updated");
                            }
                        }

                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{stocks[i].StockId} : {ex.Message}");
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        private string GetStockIdbyString()
        {
            return $@"
select s.* from [Stocks] s 
left join (select * from [MonthData] where [Datetime] ='2018-08-01')  a 
on s.StockId = a.StockId 
where a.董監持股增減 is null and s.Status = 1
order by s.StockId desc
";
        }
    }
}
