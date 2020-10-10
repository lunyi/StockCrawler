using DataService.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
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
                var trs = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div/div/table/tbody[1]/tr"));

                try
                {
                    for (int j = 1; j < trs.Count; j++)
                    {
                        var tds = trs[j].FindElements(By.XPath("td"));
                        var date = Convert.ToDateTime(tds[0].Text + "/01");
                        var monthData = context.MonthData.FirstOrDefault(p => p.StockId == stocks[i].StockId && p.Datetime == date);

                        if (monthData != null)
                        {
                            if (tds[1].Text == "-")
                            {
                                continue;
                            }
                            monthData.Close = Convert.ToDecimal(tds[1].Text.Replace(",", ""));
                            monthData.Percent = Convert.ToDecimal(tds[3].Text);
                            monthData.董監持股比例 = Convert.ToDecimal(tds[16].Text);
                            monthData.董監持股增減 = Convert.ToDecimal(tds[17].Text);
                            Console.WriteLine($"{stocks[i].StockId} Updated");
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

        private string GetStockIdbyString()
        {
            return $@"
select s.* from [Stocks] s 
left join (select * from [MonthData] where [Datetime] ='2020-08-01')  a 
on s.StockId = a.StockId 
where a.董監持股增減 is null and s.Status = 1
order by s.StockId
";
        }
    }
}
