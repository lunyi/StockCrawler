using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace WebAutoCrawler
{
    public class StockPrintCrawler : BaseCrawler2
    {
        public StockPrintCrawler() : base()
        {
        }

        private Dictionary<int, string> _types = new Dictionary<int, string>
            {
                { 1, "daily"},
                { 2, "weekly"},
                { 3, "monthly"},
                { 4, "five-minutes"},
                { 5, "quarter-hour"},
                { 6, "half-hour"},
                { 7, "hour"}
            };

        public async Task ExecuteAsync(int type)
        {
            var sw = Stopwatch.StartNew();
            sw.Start();
            //var path = $"G:\\Deploy\\BlazorWeb\\wwwroot\\photo\\{DateTime.Now:yyyy-MM-dd}";
            var path = $"D:\\Deploy\\photo\\{DateTime.Now:yyyy-MM-dd}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var t in _types)
                {
                    Directory.CreateDirectory($"{path}\\{t.Value}");
                }
            }

            var context = new StockDbContext();
            var stocks = await context.Stocks
                .Where(p => p.Status == 1)
                .OrderByDescending(p=>p.StockId)
                .ToArrayAsync();

            //int start = (index - 1) * stocks.Length / partition;
            //int end = index * stocks.Length / partition;

            //var seq = 0;
            for (int i = 0; i < stocks.Length; i++)
            {
                try
                {
                    var tagetPath = $"{path}\\{_types[type]}\\{stocks[i].StockId}.png";
                    Console.WriteLine($"{type}  {stocks[i].StockId} {stocks[i].Name}");
                    if (File.Exists(tagetPath))
                        continue;

                    //var url = "https://www.cmoney.tw/finance/stockmainkline.aspx?s=1101";
                    var url = $"https://www.wantgoo.com/stock/{stocks[i].StockId}/technical-chart";
                    GoToUrl(url);

                    Thread.Sleep(2000);

                    var element = FindElement(By.Id("technical-chart"));
                    Console.WriteLine($"找技術圖形");

                    var ele = FindElement(By.XPath($"//*[@id=\"candlestick-types\"]/li[{type}]/button"));
                    Console.WriteLine($"找週期年月日分");

                    ele.Click();
                    Thread.Sleep(1000);
                    var ss = FindElements(By.ClassName($"highcharts-button"));
                    Console.WriteLine($"全部時間找三選一");
                    ss[2].Click();
                    Thread.Sleep(1000);
                    CloseDialog();
                    GetJavaScriptExecutor().ExecuteScript(String.Format("window.scrollTo({0}, {1})", 0, element.Location.Y - 100));
                    GetScreenshot().SaveAsFile(tagetPath, ScreenshotImageFormat.Png);
                    Console.WriteLine($"{type}  {stocks[i].StockId} {stocks[i].Name} copied");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stocks[i].StockId} : {ex.Message}");
                    Dispose();
                    Intial();
                }
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMinutes);  
        }

        private void CloseDialog()
        {
            try
            {
                var close2 = FindElement(By.ClassName("_hj-2SATB__styles__minimized"));
            }
            catch (Exception)
            {
                try
                {
                    var close3 = FindElement(By.XPath("/html/body/div[5]/div/div/button"));
                    close3.Click();
                }
                catch (Exception)
                { 
                }
            }
        }
    }
}
