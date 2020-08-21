using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using DataService.Models;
using OpenQA.Selenium;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;

namespace WebAutoCrawler
{
    public class DailyTraderCrawler : BaseCrawler
    { 
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;

        public DailyTraderCrawler(LineNotifyBotApi lineNotifyBotApi) : base()
        {
            _lineNotifyBotApi = lineNotifyBotApi;
        }
        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
            var s = Stopwatch.StartNew();
            s.Start();

            await ParserAsync(context);
            //await ParserKDAsync(context);
            await ParserMACDAsync(context);

            //string url1 = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E5%9D%87%E7%B7%9A%E6%AD%A3%E4%B9%96%E9%9B%A2+%285%E6%97%A5MA%29%40%40%E5%9D%87%E7%B7%9A%E6%AD%A3%E4%B9%96%E9%9B%A2%40%405%E6%97%A5MA";
            //string url2 = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E5%9D%87%E7%B7%9A%E8%B2%A0%E4%B9%96%E9%9B%A2+%285%E6%97%A5MA%29%40%40%E5%9D%87%E7%B7%9A%E8%B2%A0%E4%B9%96%E9%9B%A2%40%405%E6%97%A5MA";
            //await ParserMAAsync(context, url1);
            //await ParserMAAsync(context, url2);

            //var prices = context.Prices.Where(P => P.Datetime == DateTime.Today)
            //    .OrderByDescending(p => p.當沖比例).Take(20).ToArray();

            //var msg = new StringBuilder();
            //msg.AppendLine($"當沖比例 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            //var index = 1;
            //foreach (var price in prices)
            //{
            //    msg.AppendLine($"{index}. {price.StockId} {price.Name} {price.當沖比例}%");
            //    index++;
            //}

            //await NotifyBotApiAsync(msg.ToString());
            //s.Stop();
            Console.WriteLine(s.Elapsed.TotalMinutes);
        }

        private async Task ParserKDAsync(StockDbContext contet)
        {
            string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E6%97%A5RSV+%28%E4%BD%8E%E2%86%92%E9%AB%98%29%40%40%E6%97%A5KD%E6%8C%87%E6%A8%99%40%40RSV+%28%E4%BD%8E%E2%86%92%E9%AB%98%29";
            GoToUrl(url);
            Thread.Sleep(5000);

            var ss = new SelectElement(FindElement(By.Id("selRANK")));
            int count = ss.Options.Count;

            for (int i = 0; i < count; i++)
            {
                var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                selRANK.SelectByIndex(i);
                Thread.Sleep(10000);
                var tables = FindElements(By.XPath($"/html/body/table[5]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                foreach (var table in tables)
                {
                    var tr = table.FindElements(By.TagName("tr"));

                    foreach (var t in tr)
                    {
                        var td = t.FindElements(By.TagName("td"));
                        try
                        {
                            var datetime = Convert.ToDateTime($"{DateTime.Now.Year}/{td[3].Text}");
                            var stockId = Convert.ToString(td[1].Text);

                            var price = contet.Prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                            if (price == null)
                                continue;

                            Console.WriteLine(td[0].Text + " " + td[1].Text + " " + td[2].Text);
                            var name = Convert.ToString(td[2].Text);
                            price.RSV = Convert.ToDecimal(td[7].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.K = Convert.ToDecimal(td[8].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.D = Convert.ToDecimal(td[9].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.RSV1 = td[7].Text;
                            price.K1 = td[8].Text;
                            price.D1 = td[9].Text;

                            await contet.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        private async Task ParserMAAsync(StockDbContext contet, string url)
        {
            GoToUrl(url);
            Thread.Sleep(5000);

            var selSHEET2 = new SelectElement(FindElement(By.Id("selSHEET2")));
            selSHEET2.SelectByIndex(0);
            Thread.Sleep(5000);

            var ss = new SelectElement(FindElement(By.Id("selRANK")));
            int count = ss.Options.Count;

            for (int i = 0; i < count; i++)
            {
                var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                selRANK.SelectByIndex(i);
                Thread.Sleep(5000);
                var tables = FindElements(By.XPath($"/html/body/table[5]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                foreach (var table in tables)
                {
                    var tr = table.FindElements(By.TagName("tr"));

                    foreach (var t in tr)
                    {
                        var td = t.FindElements(By.TagName("td"));
                        try
                        {
                            var datetime = Convert.ToDateTime($"{DateTime.Now.Year}/{td[3].Text}");
                            var stockId = Convert.ToString(td[1].Text);

                            var price = contet.Prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                            if (price == null)
                                continue;

                            Console.WriteLine(td[0].Text + " " + td[1].Text + " " + td[2].Text);
                            var name = Convert.ToString(td[2].Text);
                            price.MA5_ = td[8].Text;
                            price.MA10_ = td[9].Text;
                            price.MA20_ = td[10].Text;
                            price.MA60_ = td[11].Text;
                            await contet.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        private async Task ParserMACDAsync(StockDbContext contet)
        {
            string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E6%97%A5DIF+%28%E4%BD%8E%E2%86%92%E9%AB%98%29%40%40%E6%97%A5MACD%40%40DIF+%28%E4%BD%8E%E2%86%92%E9%AB%98%29";
            GoToUrl(url);
            Thread.Sleep(5000);

            var ss = new SelectElement(FindElement(By.Id("selRANK")));
            int count = ss.Options.Count;

            for (int i = 0; i < count; i++)
            {
                var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                selRANK.SelectByIndex(i);
                Thread.Sleep(10000);
                var tables = FindElements(By.XPath($"/html/body/table[5]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                foreach (var table in tables)
                {
                    var tr = table.FindElements(By.TagName("tr"));

                    foreach (var t in tr)
                    {
                        var td = t.FindElements(By.TagName("td"));
                        try
                        {
                            var datetime = Convert.ToDateTime($"{DateTime.Now.Year}/{td[3].Text}");
                            var stockId = Convert.ToString(td[1].Text);

                            var price = contet.Prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                            if (price == null)
                                continue;

                            Console.WriteLine(td[0].Text + " " + td[1].Text + " " + td[2].Text);
                            var name = Convert.ToString(td[2].Text);
                            price.DIF = Convert.ToDecimal(td[7].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.MACD = Convert.ToDecimal(td[8].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.OSC = Convert.ToDecimal(td[9].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.DIF1 = td[7].Text;
                            price.MACD1 = td[8].Text;
                            price.OSC1 = td[9].Text;

                            await contet.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        private async Task ParserAsync(StockDbContext contet)
        {
            string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E7%8F%BE%E8%82%A1%E7%95%B6%E6%B2%96%E5%BC%B5%E6%95%B8+%28%E7%95%B6%E6%97%A5%29%40%40%E7%8F%BE%E8%82%A1%E7%95%B6%E6%B2%96%E5%BC%B5%E6%95%B8%40%40%E7%95%B6%E6%97%A5";
            GoToUrl(url);
 
            //for (int k = 20; k >= 2; k--)
            {
                //var selRPT_TIME = new SelectElement(FindElement(By.Id("selRPT_TIME")));
                //selRPT_TIME.SelectByIndex(k);
                Thread.Sleep(5000);

                for (int i = 0; i <= 5; i++)
                {
                    var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                    selRANK.SelectByIndex(i);
                    Thread.Sleep(10000);

                    var tables = FindElements(By.XPath($"/html/body/table[5]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                    foreach (var table in tables)
                    {
                        var tr = table.FindElements(By.TagName("tr"));

                        foreach (var t in tr)
                        {
                            var td = t.FindElements(By.TagName("td"));

                            if (td[10].Text.Trim() == "-")
                                continue;

                            var year = Convert.ToInt16(td[10].Text.Substring(0, 2)) >= 10 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                            try
                            {
                                var datetime = Convert.ToDateTime($"{year}/{td[10].Text}");
                                var stockId = Convert.ToString(td[1].Text);

                                var price = contet.Prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                                if (price == null)
                                    continue;

                                Console.WriteLine(td[10].Text + "　" +td[0].Text + " " + td[1].Text + " " + td[2].Text);
                                var name = Convert.ToString(td[2].Text);
                                price.當沖張數 = Convert.ToInt32(td[11].Text.Replace(",", ""));
                                price.當沖比例 = Convert.ToDecimal(td[12].Text);
                                price.當沖總損益 = Convert.ToDecimal(td[17].Text);
                                price.當沖均損益 = td[18].Text == "" ? 0 : Convert.ToDecimal(td[18].Text);
                                await contet.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                }
            }
        }

        private async Task NotifyBotApiAsync(string message)
        {
            await _lineNotifyBotApi.Notify(new NotifyRequestDTO
            {
                AccessToken = _token,
                Message = message
            });
        }

        private class TempStock
        {
            public string StockId { get; set; }
            public string Name { get; set; }
            public DateTime Datetime { get; set; }
            public int 當沖張數 { get; set; }
            public decimal 當沖比例 { get; set; }
            public decimal 當沖總盈虧 { get; set; }
            public decimal 當沖均盈虧 { get; set; }
        }
    }
}
