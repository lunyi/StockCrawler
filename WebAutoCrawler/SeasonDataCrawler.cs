using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using OpenQA.Selenium;

namespace WebAutoCrawler
{
    public class SeasonDataCrawler : BaseCrawler
    {
        string HealthCheckUrl = "https://www.cmoney.tw/finance/f00040.aspx?s={0}";
        string FinanceUrl = "https://www.cmoney.tw/finance/f00043.aspx?s={0}";

        public SeasonDataCrawler() : base()
        {
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
                    var season = Parser(context, string.Format(HealthCheckUrl, stock.StockId), stock.StockId, stock.Name);
                    context.SeasonData.AddRange(season);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} {ex} !");
                }
            }
        }

        private SeasonData[] Parser(StockDbContext context, string url, string stockId, string name)
        {
            GoToUrl(url);
            var table = FindElement(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table"));
            var test = FindElement(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[1]"));
            var season   = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[1]/th"));
            var 資產累計 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[10]/td"));
            var 負債 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[15]/td"));
            var 股本 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[16]/td"));
            var 股東權益 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[17]/td"));
            var 公告每股淨值 = FindElements(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table/tbody/tr[18]/td"));

            var seasonData = new List<SeasonData>();
            for (var i = 1; i < season.Count; i++)
            {
                var q = Convert.ToInt32(season[i].Text.Substring(5, 1)) * 3;
                var datetime = Convert.ToDateTime(season[i].Text.Substring(0, 4) + "-" + q.ToString("00") + "-01");

                if (context.SeasonData.Any(p => p.StockId == stockId && p.Datetime == datetime))
                    continue;

                seasonData.Add(new SeasonData
                {
                    Id = Guid.NewGuid(),
                    StockId = stockId,
                    Name = name,
                    CreatedOn = DateTime.Now,
                    Datetime = datetime,
                    資產總計 = Convert.ToDecimal(資產累計[i].Text.Replace(",", "")),
                    負債總計 = Convert.ToDecimal(負債[i].Text.Replace(",", "")),
                    股本 = Convert.ToDecimal(股本[i].Text.Replace(",", "")),
                    股東權益 = Convert.ToDecimal(股東權益[i].Text.Replace(",", "")),
                    公告每股淨值 = Convert.ToDecimal(公告每股淨值[i].Text.Replace(",", ""))
                });          
            }

            ParserFinance(stockId, seasonData);
            return seasonData.OrderByDescending(p=>p.Datetime).ToArray();
        }

        private SeasonData[] ParserFinance(string stockId, List<SeasonData> data)
        {
            GoToUrl(string.Format(FinanceUrl, stockId));
            var table = FindElement(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table"));
            var title = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[1]/th"));
            var 毛利率 = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[2]/td"));
            var 營業利益率 = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[3]/td"));
            var ROE = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[6]/td"));
            var ROA = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[7]/td"));
            var 每股營業額 = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[8]/td"));
            var 每股稅後盈餘 = FindElements(By.XPath("//*[@id='MainContent']/ul/li[2]/article/div/div/div/table/tbody/tr[10]/td"));

            for (int k = 0; k < data.Count; k++)
            {
                var i = k + 1;
                var t = title[i].Text;
                var q = Convert.ToInt32(title[i].Text.Substring(5, 1)) * 3;
                var datetime = Convert.ToDateTime(title[i].Text.Substring(0, 4) + "-" + q.ToString("00") + "-01");
                var season = data.FirstOrDefault(p => p.Datetime == datetime);

                if (season != null)
                {
                    var ss = 毛利率[i].Text;
                    season.毛利率 = Convert.ToDecimal(毛利率[i].Text);
                    season.營業利益率 = Convert.ToDecimal(營業利益率[i].Text);
                    season.ROE = Convert.ToDecimal(ROE[i].Text);
                    season.ROA = Convert.ToDecimal(ROA[i].Text);
                    season.每股營業額 = Convert.ToDecimal(每股營業額[i].Text);
                    season.每股稅後盈餘 = Convert.ToDecimal(每股稅後盈餘[i].Text);
                }
            }
            return data.ToArray();
        }
    }
}
