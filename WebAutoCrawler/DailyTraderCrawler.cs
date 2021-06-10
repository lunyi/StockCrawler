using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebAutoCrawler
{
    public class DailyTraderCrawler : BaseCrawler2
    {
        public async Task ExecuteAsync(string type)
        {
            if (!funcMap.Keys.Any(p => p == type))
            {
                Console.WriteLine("不合法參數");
                base.Dispose();
                return;
            }

            var s = Stopwatch.StartNew();
            s.Start();

            var context = new StockDbContext();

            var prices = await context.Prices
                .Where(p => p.Datetime == DateTime.Today)
                .ToListAsync();
            var func = funcMap[type];

            var pricesToUpdate = func(prices);

            context.Database.SetCommandTimeout(300);

            try
            {
                await context.BulkUpdateAsync(pricesToUpdate);
                context.Database.ExecuteSqlRaw($"exec [usp_Update_MA_And_VMA] {DateTime.Today:yyyy-MM-dd}");
                Console.WriteLine(s.Elapsed.TotalMinutes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e}");
            }

            base.Dispose();
        }

        static Dictionary<string, Func<List<Prices>, List<Prices>>> funcMap = new Dictionary<string, Func<List<Prices>, List<Prices>>>
        {
            { "dailytrade", (p) => dailyTraderFunc(p)},
            { "macd", (p) => dailyMacdFunc(p)},
            { "kd", (p) => dailyKdFunc(p)},
            { "ma", (p) => dailyMAFunc(p)},
        };

        static Func<List<Prices>, List<Prices>> dailyTraderFunc = (prices) =>
            {
                string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E7%8F%BE%E8%82%A1%E7%95%B6%E6%B2%96%E5%BC%B5%E6%95%B8+%28%E7%95%B6%E6%97%A5%29%40%40%E7%8F%BE%E8%82%A1%E7%95%B6%E6%B2%96%E5%BC%B5%E6%95%B8%40%40";
                var updatedPrices = new List<Prices>();

                GoToUrl(url);
                Thread.Sleep(5000);

                var s1 = new SelectElement(FindElement(By.XPath($@"//*[@id='MENU8_0_1100']/tbody/tr[6]/td[1]/nobr[3]/select")));
                s1.SelectByIndex(1);
                Thread.Sleep(1000);

                for (int i = 0; i <= 5; i++)
                {
                    var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                    selRANK.SelectByIndex(i);
                    Thread.Sleep(5000);

                    var tables = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                    foreach (var table in tables)
                    {
                        var tr = table.FindElements(By.TagName("tr"));

                        foreach (var t in tr)
                        {
                            try
                            {
                                var td = t.FindElements(By.TagName("td"));

                                //for (int k = 0; k < td.Count; k++)
                                //{
                                //    Console.WriteLine($"td[{k}].Text = {td[k].Text}");
                                //}      

                                if (td[1].Text == "代號")
                                    continue;
                                if (td.Count >= 10 && td[10].Text.Trim() == "-")
                                    continue;
                                if (td.Count <= 3)
                                    continue;

                                var year = DateTime.Now.Year;
                         
                                var datetime = Convert.ToDateTime($"{year}/{td[4].Text}");
                                var stockId = Convert.ToString(td[1].Text);
                                var updatedPrice = prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                                if (updatedPrice == null)
                                    continue;

                                Console.WriteLine($"DailyTrader {td[10].Text} {td[0].Text} {td[1].Text} {td[2].Text}");

                                var name = Convert.ToString(td[2].Text);
                                updatedPrice.當沖張數 = Convert.ToInt32(td[11].Text.Replace(",", ""));
                                updatedPrice.當沖比例 = Convert.ToDecimal(td[12].Text);
                                updatedPrice.當沖總損益 = Convert.ToDecimal(td[17].Text);
                                updatedPrice.當沖均損益 = td[18].Text == "" ? 0 : Convert.ToDecimal(td[18].Text);
                                updatedPrices.Add(updatedPrice);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                }

                return updatedPrices;
            };

        static Func<List<Prices>, List<Prices>> dailyMAFunc = (prices) =>
            {
                string maUrl1 = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E5%9D%87%E7%B7%9A%E6%AD%A3%E4%B9%96%E9%9B%A2+%2810%E6%97%A5MA%29%40%40%E5%9D%87%E7%B7%9A%E6%AD%A3%E4%B9%96%E9%9B%A2%40%4010%E6%97%A5MA";
                string maUrl2 = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E5%9D%87%E7%B7%9A%E8%B2%A0%E4%B9%96%E9%9B%A2+%2810%E6%97%A5MA%29%40%40%E5%9D%87%E7%B7%9A%E8%B2%A0%E4%B9%96%E9%9B%A2%40%4010%E6%97%A5MA";

                var tmp1 = dailyMaFunc(maUrl1,1, prices);
                var tmp2 = dailyMaFunc(maUrl2,2, prices);
                return tmp1.Union(tmp2).ToList();
            };

        static Func<string, int, List<Prices>, List<Prices>> dailyMaFunc = (url, index, prices) =>
        {
            //prices = prices.Where(p => p.MA10_ == null).ToList();
            var updatedPrices = new List<Prices>();

            GoToUrl(url);
            Thread.Sleep(5000);

            var s1 = new SelectElement(FindElement(By.XPath($@"//*[@id='MENU8_1_1100']/tbody/tr[2]/td[{index}]/nobr[3]/select")));
            s1.SelectByIndex(2);
            Thread.Sleep(2000);
            s1.SelectByIndex(1);
            Thread.Sleep(2000);

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
                var tables = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));

                for (int j = 0; j < tables.Count; j++)
                {
                    var tr = tables[j].FindElements(By.TagName("tr"));
                    for (int k = 0; k < tr.Count; k++)
                    {
                        var td = tr[k].FindElements(By.TagName("td"));
                        try
                        {
                            if (td.Count <= 3)
                                continue;
                            if (td[1].Text == "代號")
                                continue;

                            var date = td[7].Text;
                            var datetime = Convert.ToDateTime($"{DateTime.Now.Year}/{date}");
                            var stockId = Convert.ToString(td[1].Text);

                            var price = prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                            if (price == null)
                                continue;

                            Console.WriteLine($"MA {td[0].Text} {td[1].Text} {td[2].Text}");

                            var name = Convert.ToString(td[2].Text);
                            price.MA5_ = td[8].Text;
                            price.MA10_ = td[9].Text;
                            price.MA20_ = td[10].Text;
                            price.MA60_ = td[11].Text;
                            updatedPrices.Add(price);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            return updatedPrices;
        };

        static Func<List<Prices>, List<Prices>> dailyMacdFunc = (prices) =>
        {
            //prices = prices.Where(p => p.MACD == null).ToList();
            string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E6%97%A5DIF+%28%E9%AB%98%E2%86%92%E4%BD%8E%29%40%40%E6%97%A5MACD%40%40DIF+%28%E9%AB%98%E2%86%92%E4%BD%8E%29";
            var updatedPrices = new List<Prices>();

            GoToUrl(url);
            Thread.Sleep(5000);

            var s1 = new SelectElement(FindElement(By.XPath($@"//*[@id='MENU8_1_1100']/tbody/tr[5]/td[1]/nobr[3]/select")));
            s1.SelectByIndex(2);
            Thread.Sleep(2000);
            s1.SelectByIndex(1);
            Thread.Sleep(2000);

            var ss = new SelectElement(FindElement(By.Id("selRANK")));
            int count = ss.Options.Count;

            for (int i = 0; i < count; i++)
            {
                var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                selRANK.SelectByIndex(i);
                Thread.Sleep(5000);
                var tables = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                foreach (var table in tables)
                {
                    var tr = table.FindElements(By.TagName("tr"));

                    foreach (var t in tr)
                    {
                        var td = t.FindElements(By.TagName("td"));
                        try
                        {
                            if (td.Count <= 3)
                                continue;
                            if (td[1].Text == "代號")
                                continue;
                            var datetime = Convert.ToDateTime($"{DateTime.Now.Year}/{td[6].Text}");
                            var stockId = Convert.ToString(td[1].Text);

                            var price = prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                            if (price == null)
                                continue;

                            Console.WriteLine($"MACD {td[0].Text} {td[1].Text} {td[2].Text}");

                            var name = Convert.ToString(td[2].Text);
                            price.DIF = Convert.ToDecimal(td[7].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.MACD = Convert.ToDecimal(td[8].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.OSC = Convert.ToDecimal(td[9].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.DIF1 = td[7].Text;
                            price.MACD1 = td[8].Text;
                            price.OSC1 = td[9].Text;

                            updatedPrices.Add(price);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            return updatedPrices;
        };

        static Func<List<Prices>, List<Prices>> dailyKdFunc = (prices) =>
        {
            //prices = prices.Where(p => p.K == null).ToList();
            string url = "https://goodinfo.tw/StockInfo/StockList.asp?RPT_TIME=&MARKET_CAT=%E7%86%B1%E9%96%80%E6%8E%92%E8%A1%8C&INDUSTRY_CAT=%E6%97%A5RSV+%28%E9%AB%98%E2%86%92%E4%BD%8E%29%40%40%E6%97%A5KD%E6%8C%87%E6%A8%99%40%40RSV+%28%E9%AB%98%E2%86%92%E4%BD%8E%29";
            var updatedPrices = new List<Prices>();

            GoToUrl(url);
            Thread.Sleep(5000);

            var s1 = new SelectElement(FindElement(By.XPath($@"//*[@id='MENU8_1_1100']/tbody/tr[4]/td[1]/nobr[3]/select")));
            s1.SelectByIndex(2);
            Thread.Sleep(2000);
            s1.SelectByIndex(1);
            Thread.Sleep(2000);

            var ss = new SelectElement(FindElement(By.Id("selRANK")));
            int count = ss.Options.Count;

            for (int i = 0; i < count; i++)
            {
                var selRANK = new SelectElement(FindElement(By.Id("selRANK")));
                selRANK.SelectByIndex(i);
                Thread.Sleep(5000);
                var tables = FindElements(By.XPath($"/html/body/table[2]/tbody/tr/td[3]/div[2]/div/div/table/tbody"));
                foreach (var table in tables)
                {
                    var tr = table.FindElements(By.TagName("tr"));

                    foreach (var t in tr)
                    {
                        var td = t.FindElements(By.TagName("td"));
                        try
                        {
                            if (td.Count <= 3)
                                continue;
                            if (td[1].Text == "代號")
                                continue;
                            var datetime = Convert.ToDateTime($"{DateTime.Now.Year}/{td[6].Text}");
                            var stockId = Convert.ToString(td[1].Text);

                            var price = prices.FirstOrDefault(p => p.Datetime == datetime && p.StockId == stockId);

                            if (price == null)
                                continue;

                            Console.WriteLine($"KD {td[0].Text} {td[1].Text} {td[2].Text}");
                            var name = Convert.ToString(td[2].Text);
                            price.RSV = Convert.ToDecimal(td[7].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.K = Convert.ToDecimal(td[8].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.D = Convert.ToDecimal(td[9].Text.Replace("↘", "").Replace("↗", "").Replace("→", ""));
                            price.RSV1 = td[7].Text;
                            price.K1 = td[8].Text;
                            price.D1 = td[9].Text;
                            updatedPrices.Add(price);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            return updatedPrices;
        };
    }
}
