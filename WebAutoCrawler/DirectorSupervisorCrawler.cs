using DataService.Models;
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

            
            var stocks = context.Stocks.Where(p => p.Status == 1).ToArray();

            Thread.Sleep(5000);

            for (int i = 0; i <= stocks.Length; i++)
            {
                var url = $"https://goodinfo.tw/StockInfo/StockDirectorSharehold.asp?STOCK_ID={stocks[i].StockId}";
                GoToUrl(url);
                Thread.Sleep(1000);

                var tables1 = FindElement(By.XPath($"/html/body/table[2]"));



                var tables = FindElements(By.XPath($"/html/body/table[2]/tr/td[3]/div/div/table/tr"));
               
            }
        }
    }
}
