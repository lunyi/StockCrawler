﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace WebAutoCrawler
{
    public class CMoneyCrawler : BaseCrawler
    {
        string HealthCheckUrl = "https://www.cmoney.tw/finance/f00025.aspx?s={0}";
        string BillionUrl = "https://www.cmoney.tw/finance/f00038.aspx?s={0}";
        public CMoneyCrawler() : base()
        {

        }

        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            //var s = context.Stocks.FromSqlRaw(GetSql()).ToList();
            //var stocks = context.Stocks.FromSqlRaw(GetSql()).ToList();
            var stocks = context.Stocks.Where(p=>p.Status == 1).OrderBy(p=>p.StockId).ToList();

            foreach (var stock in stocks)
            {
                try
                {
                    GoToUrl(string.Format(HealthCheckUrl, stock.StockId));

                    Thread.Sleep(400);
                    var checks = FindElement(By.ClassName("remark"));
                    var barnums = FindElements(By.ClassName("bar-num2"));

                    var item = new AnaCMoney
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.StockId,
                        Name = stock.Name,
                        Remark = checks.Text,
                        價值 = Convert.ToInt32(barnums[0].Text),
                        安全 = Convert.ToInt32(barnums[1].Text),
                        成長 = Convert.ToInt32(barnums[2].Text),
                        籌碼 = Convert.ToInt32(barnums[3].Text),
                        技術 = Convert.ToInt32(barnums[4].Text),
                        CreatedOn = DateTime.Now,
                    };
                    context.AnaCMoney.Add(item);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
                }
            }
        }

        public async Task ExecuteBillionAsync()
        {
            var context = new StockDbContext();
            //var s = context.Stocks.FromSqlRaw(GetSql()).ToList();
            //var stocks = context.Stocks.FromSqlRaw(GetSql2()).ToList();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p=>p.StockId).ToList();

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
                            Id = Guid.NewGuid(),
                            StockId = stock.StockId,
                            Name = stock.Name,       
                            Datetime = Convert.ToDateTime(titles[i].Text + "/01"),
                            //PercentOver400= Convert.ToDecimal(overFourHundreds[i].Text),                       
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

                        context.Thousand.Add(s);
                    }

                    await context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
                }
            }
        }

        public async Task ExecuteFinanceAsync()
        {
            string financeUrl = "https://www.cmoney.tw/finance/f00040.aspx?s={0}&o=3";

            var context = new StockDbContext();
            //var s = context.Stocks.FromSqlRaw(GetSql()).ToList();
            //var stocks = context.Stocks.FromSqlRaw(GetSql2()).ToList();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p=>p.StockId).ToList();

            foreach (var stock in stocks)
            {
                try
                {
                    GoToUrl(string.Format(financeUrl, stock.StockId));

                    Thread.Sleep(400);
                    var table = FindElement(By.XPath("//*[@id='MainContent']/ul/li/article/div[2]/div/table"));
                    var trs = table.FindElements(By.TagName("tr"));

                    var startYear = 2018;
                    var list = new List<YearData>();
                    for (int i = 1; i < trs.Count; i++)
                    {
                        var tds = trs[i].FindElements(By.TagName("td"));
                        var key = tds[0].Text;

                        if (key.Trim() == "流動資產")
                        {
                            for (int j = 1; j < tds.Count; j++)
                            {
                                var y = new YearData
                                {
                                    Id = Guid.NewGuid(),
                                    StockId = stock.StockId,
                                    Name = stock.Name,
                                    Year = startYear + 1 - j,
                                    流動資產 = Convert.ToDecimal(tds[j].Text.Replace(",", "")),
                                    CreatedOn = DateTime.Now,
                                };
                                list.Add(y);
                            }
                        }
                        if (key.Trim() == "流動負債")
                        {
                            for (int j = 1; j < tds.Count; j++)
                            {
                                var year = startYear + 1 - j;
                                var data = list.FirstOrDefault(p => p.Year == year);
                                data.流動負債 = Convert.ToDecimal(tds[j].Text.Replace(",", ""));
                            }
                        }

                        if (key.Trim() == "資產總計")
                        {
                            for (int j = 1; j < tds.Count; j++)
                            {
                                var year = startYear + 1 - j;
                                var data = list.FirstOrDefault(p => p.Year == year);
                                data.資產總計 = Convert.ToDecimal(tds[j].Text.Replace(",", ""));
                            }
                        }

                        if (key.Trim() == "負債總計")
                        {
                            for (int j = 1; j < tds.Count; j++)
                            {
                                var year = startYear + 1 - j;
                                var data = list.FirstOrDefault(p => p.Year == year);
                                data.負債總計 = Convert.ToDecimal(tds[j].Text.Replace(",", ""));
                            }
                        }
                        if (key.Trim() == "股東權益")
                        {
                            for (int j = 1; j < tds.Count; j++)
                            {
                                var year = startYear + 1 - j;
                                var data = list.FirstOrDefault(p => p.Year == year);
                                data.股東權益 = Convert.ToDecimal(tds[j].Text.Replace(",", ""));
                            }
                        }
                        if (key.Trim() == "公告每股淨值")
                        {
                            for (int j = 1; j < tds.Count; j++)
                            {
                                var year = startYear + 1 - j;
                                var data = list.FirstOrDefault(p => p.Year == year);
                                data.公告每股淨值 = Convert.ToDecimal(tds[j].Text.Replace(",", ""));
                            }
                        }
                    }

                    context.YearData.AddRange(list);
                    await context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
                }
            }
        }

        string GetSql()
        {
            return @"

              SELECT a.*
  FROM [dbo].[Stocks] a 
  left join (
  SELECT *
  FROM [StockDb].[dbo].[AnaCMoney]
  where CreatedOn > '2019-10-07'
  ) b on a.StockID = b.StockId
  where b.StockId is null and a.Status = 1
  order by StockId
            ";
        }

        string GetSql2()
        {
            return $@"
select * from [Stocks]
  where [Status] = 1 and StockId not in (
  select StockId
  FROM [StockDb].[dbo].[Thousand])
  order by StockId
";
        }
    }
}
