using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Messages;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace WebAutoCrawler
{
    public class MonthDataCrawler : BaseCrawler
    {
        string HealthCheckUrl = "https://histock.tw/stock/financial.aspx?no={0}";

        public MonthDataCrawler() : base()
        {
        }
        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            var date = $@"{DateTime.Today.AddDays(-30).ToString("yyyy-MM-01")}";
            var stocks = context.Stocks.FromSqlRaw(GetSql(date)).ToList();

            var allMonthData = context.MonthData.ToList();

            foreach (var stock in stocks)
            {
                var url = string.Format(HealthCheckUrl, stock.StockId);
                try
                {
                    //var monthData = Parser(string.Format(HealthCheckUrl, stock.StockId), stock.StockId, stock.Name);
                    //context.MonthData.AddRange(monthData);
                    //await context.SaveChangesAsync();

                    var monthData = ParserLatest(url, stock.StockId, stock.Name, allMonthData);

                    if (monthData != null)
                    {
                        context.MonthData.Add(monthData);
                        await context.SaveChangesAsync();
                    }              
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{url} !");                  
                    Console.WriteLine($"{stock.StockId} {stock.Name} {ex} !");
                }
            }
        }

        private string GetSql(string datetime)
        {
            return @$"
    select s.* from [Stocks]  s 
  left join (select * from [MonthData] where [Datetime] = '{datetime}') p on s.StockId = p.StockId
  where  s.Status = 1 and p.Id is null
  order by s.StockId
";
        }
        private MonthData ParserLatest(string url, string stockId, string name, List<MonthData> monthData)
        {
            GoToUrl(url);

            var table = FindElement(By.XPath("//*[@id='form1']/div[4]/div[4]/div/div[1]/div[3]/div/div[5]/div/table"));
                                              //*[@id="form1"]/div[4]/div[4]/div/div[1]/div[2]/div/div[5]/div/table/tbody/tr[3]

            var rows = table.FindElements(By.TagName("tr"));
            var tds = rows[2].FindElements(By.TagName("td"));
            var datetime = Convert.ToDateTime(tds[0].Text + "/01");
            var newMonth = monthData.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

            if (newMonth == null &&  tds[0].Text == DateTime.Now.AddDays(-30).ToString("yyyy/MM"))
            {
                Console.WriteLine($"{stockId} {name} {tds[0].Text} Added");

                return new MonthData
                {
                    Id = Guid.NewGuid(),
                    StockId = stockId,
                    Name = name,
                    CreatedOn = DateTime.Now,
                    Datetime = Convert.ToDateTime(tds[0].Text + "/01"),
                    單月營收 = Convert.ToDecimal(tds[1].Text.Replace(",", "")),
                    去年同月營收 = Convert.ToDecimal(tds[2].Text.Replace(",", "")),
                    單月月增率 = Convert.ToDecimal(tds[3].Text.Replace("%", "")),
                    單月年增率 = Convert.ToDecimal(tds[4].Text.Replace("%", "")),
                    累計營收 = Convert.ToDecimal(tds[5].Text.Replace(",", "")),
                    去年累計營收 = Convert.ToDecimal(tds[6].Text.Replace(",", "")),
                    累積年增率 = Convert.ToDecimal(tds[7].Text.Replace("%", "")),
                };
            }
            return null;
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
