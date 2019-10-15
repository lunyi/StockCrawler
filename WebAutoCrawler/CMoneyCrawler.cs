﻿using System;
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
        public CMoneyCrawler() : base()
        {

        }

        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            //var s = context.Stocks.FromSqlRaw(GetSql()).ToList();
            var stocks = context.Stocks.FromSqlRaw(GetSql()).ToList();

            foreach (var stock in stocks)
            {
                try
                {
                    _driver.Navigate().GoToUrl(string.Format(HealthCheckUrl, stock.StockId));

                    Thread.Sleep(400);
                    var checks = _driver.FindElement(By.ClassName("remark"));
                    var barnums = _driver.FindElements(By.ClassName("bar-num2"));

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
    }
}
