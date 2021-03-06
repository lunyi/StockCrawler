﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebAutoCrawler
{
    public class ThousandDataCrawlerV2 : BaseCrawler
    {
        string BillionUrl = "https://www.tdcc.com.tw/smWeb/QryStock.jsp";

        public ThousandDataCrawlerV2() : base()
        {
            GoToUrl(BillionUrl);
        }
        public async Task ExecuteHistoryAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p => p.StockId).ToArray();
            var tdatetimes = context.Thousands.Where(p=>p.Datetime >= Convert.ToDateTime("2019-08-30"))
                .Select(p => p.Datetime)
                .Distinct()
                .OrderByDescending(p=>p)
                .ToArray();
            var datetimes = tdatetimes.Select((p, i) => new TempDatetime
            { 
                Datetime = p,
                Index = i
            }).ToArray();
            var alltemp = (from s in stocks
                           from d in datetimes
                           join th in context.Thousands on new { s.StockId, d.Datetime } equals new { th.StockId, th.Datetime }
                            into thTmp
                           from t in thTmp.DefaultIfEmpty()
                           where t == null
                           select new TempThousand
                      {
                          StockId = s.StockId,
                          Name = s.Name,
                          Index = d.Index,
                          Datetime = d.Datetime
                      }).ToArray();

            foreach (var stock in alltemp)
            {
                try
                {
                    await ParserAsync(context, stock.StockId, stock.Name, stock.Index);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed {ex}!");
                }
            }

            Dispose();
        }

        [Obsolete]
        public async Task ExecuteLastAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.FromSqlRaw(GetStockIdbyString(GetCurrentDate())).ToArray();

            foreach (var stock in stocks)
            {
                try
                {
                    await ParserAsync(context, stock.StockId, stock.Name, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed {ex}!");
                }
            }

            context.Database.ExecuteSqlRaw(GetSqlToUpdate(GetCurrentDate()));

            Dispose();
        }

        private string GetCurrentDate()
        {
            GoToUrl(BillionUrl);
            Thread.Sleep(300);
            var selectElement = new SelectElement(FindElement(By.Id("scaDates")));
            var res = selectElement.Options[0].Text.Insert(6, "-").Insert(4, "-");
            return res;
        }
        private string GetStockIdbyString(string datetime)
        {
            return $@"
select s.* from [Stocks] s 
left join (select * from [Thousand] where [Datetime] = '{datetime}')  a 
on s.StockId = a.StockId 
where a.StockId is null and s.Status = 1
order by s.StockId
";
        }

        private string GetSqlToUpdate(string datetime)
        {
            return $@"
WITH TOPTEN1 as (
   SELECT *, ROW_NUMBER() 
    over (
        PARTITION BY [Name] 
       order by [Datetime] desc
    ) AS RowNo 
    FROM [Thousand]  where [Datetime] <= '{datetime}' and [Datetime] >= DATEADD(DD, -10,'{datetime}')
)

update [Thousand] set [PPUnder100] = t1.[PPUnder100], [PPOver1000] = t1.[PPOver1000]
from [Thousand] t
join (
select 
t.RowNo, t.[StockId], t.[NAme], t.[Datetime], t.[PUnder100], t.[POver1000], t1.[PUnder100] as [PPUnder100], t1.[POver1000] as [PPOver1000]
from TOPTEN1 t
join TOPTEN1 t1 on t1.StockId = t.StockId and t.RowNo + 1 = t1.RowNo ) t1 on t.StockId = t1.StockId and t.Datetime = t1.Datetime
where t.[Datetime] = '{datetime}'
";
        }

        public override async Task ExecuteAsync()
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p => p.StockId).ToArray();

            foreach (var stock in stocks)
            {
                for (int k = 0; k < 15; k++)
                {
                    try
                    {
                        await ParserAsync(context, stock.StockId, stock.Name, k);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed {ex}!");
                    }
                }
            }
        }

        private async Task ParserAsync(StockDbContext context, string stockId, string name,  int index)
        {
            GoToUrl(BillionUrl);
            Thread.Sleep(300);
            var k = index;
            var selectElement = new SelectElement(FindElement(By.Id("scaDates")));
            var date = selectElement.Options[k].Text.Insert(6, "-").Insert(4, "-");

            selectElement.SelectByIndex(k);
            FindElement(By.Id("StockNo")).SendKeys(stockId);
            FindElement(By.Name("sub")).Click();

            Console.WriteLine(stockId + "--" + date);
            Thread.Sleep(100);

            var t = new Thousand();
            t.StockId = stockId;
            t.Name = name;
            t.Datetime = Convert.ToDateTime(date);
            t.CreatedOn = DateTime.Now;

            var p = context.Thousands.FirstOrDefault(p => p.StockId == stockId && p.Datetime == t.Datetime);
            if (p != null)
            {
                return;
            }
            decimal temp = 0;
            (t.C1, t.S1, t.P1) = GetPercent(2);
            (t.C5, t.S5, t.P5) = GetPercent(3);
            (t.C10, t.S10, t.P10) = GetPercent(4);
            (t.C15, t.S15, t.P15) = GetPercent(5);
            (t.C20, t.S20, t.P20) = GetPercent(6);
            (t.C30, t.S30, t.P30) = GetPercent(7);
            (t.C40, t.S40, t.P40) = GetPercent(8);
            (t.C50, t.S50, t.P50) = GetPercent(9);
            (t.C100, t.S100, t.P100) = GetPercent(10);
            (t.C200, t.S200, t.P200) = GetPercent(11);
            (t.C400, t.S400, t.P400) = GetPercent(12);
            (t.C600, t.S600, t.P600) = GetPercent(13);
            (t.C800, t.S800, t.P800) = GetPercent(14);
            (t.C1000, t.S1000, t.P1000) = GetPercent(15);
            (t.COver1000, t.SOver1000, t.POver1000) = GetPercent(16);

            try
            {
                (t.TotalCount, t.TotalStock, temp) = GetPercent(17);
            }
            catch (Exception)
            {
                (t.TotalCount, t.TotalStock, temp) = GetPercent(18);
            }

            t.PUnder100 = t.P1 + t.P5 + t.P10 + t.P15 + t.P20 + t.P30 + t.P40 + t.P50 + t.P100;
            t.CUnder100 = t.C1 + t.C5 + t.C10 + t.C15 + t.C20 + t.C30 + t.C40 + t.C50 + t.C100;
            t.SUnder100 = t.S1 + t.S5 + t.S10 + t.S15 + t.S20 + t.S30 + t.S40 + t.S50 + t.S100;

            context.Thousands.Add(t);
            await context.SaveChangesAsync();
        }

        private (int, decimal, decimal) GetPercent(int index)
        {
            var p1 = Convert.ToInt32(FindElement(By.XPath(@$"//*[@id='Qform']/table/tbody/tr/td/table[6]/tbody/tr[{index}]/td[3]")).Text.Replace(",",""));
            var s1 = Convert.ToDecimal(FindElement(By.XPath(@$"//*[@id='Qform']/table/tbody/tr/td/table[6]/tbody/tr[{index}]/td[4]")).Text.Replace(",", ""))/1000;
            var c1 = Convert.ToDecimal(FindElement(By.XPath(@$"//*[@id='Qform']/table/tbody/tr/td/table[6]/tbody/tr[{index}]/td[5]")).Text.Replace(",", ""));
            return (p1, s1, c1);
        }
    }

    public class TempThousand 
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public DateTime Datetime { get; set; }
    }

    public class TempDatetime
    {
        public int Index { get; set; }
        public DateTime Datetime { get; set; }
    }
}
