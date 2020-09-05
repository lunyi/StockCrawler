using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace WebAutoCrawler
{
    public class StockPrintCrawler : BaseCrawler
    {
        public StockPrintCrawler() : base()
        {
        }

        public override async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            sw.Start();
            var path = $"D:\\Deploy\\BlazorWeb\\wwwroot\\photo\\{DateTime.Now:yyyy-MM-dd}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = new StockDbContext();
            var stocks = await context.Stocks
                .Where(p => p.Status == 1)
                .ToArrayAsync();

            foreach (var stock in stocks)
            {
                try
                {
                    //var url = "https://www.cmoney.tw/finance/stockmainkline.aspx?s=1101";
                    var url = $"https://www.wantgoo.com/stock/{stock.StockId}/technical-chart";
                    GoToUrl(url);

                    Console.WriteLine(stock.StockId);
                    Thread.Sleep(2000);
                    var element = FindElement(By.Id("technical-chart"));
                    GetJavaScriptExecutor().ExecuteScript(String.Format("window.scrollTo({0}, {1})", 0, element.Location.Y - 40));
                    GetScreenshot().SaveAsFile($"{path}\\{stock.StockId}.png", ScreenshotImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stock.StockId} : {ex.Message}");
                }
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMinutes);
            Console.ReadLine();
        }
    }
}
