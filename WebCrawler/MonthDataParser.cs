﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class MonthDataParser : BaseParser
    {
        public async Task RunAsync()
        {
            var context = new StockDbContext();

            var stocks = context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToList();

            var s = Stopwatch.StartNew();
            s.Start();

            foreach (var stock in stocks)
            {
                await ParserStockAsync(context, stock);
            }

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
            Console.ReadLine();
        }

        private async Task ParserStockAsync(StockDbContext context, Stocks stock)
        {
            try 
            {
                var rootNode = GetRootNoteByUrl($"https://histock.tw/stock/financial.aspx?no={stock.StockId}");
                var ss = rootNode.SelectSingleNode("//*[@id='form1']/div[4]/div[3]/div[2]/div[1]/div[1]/div/div[5]/div/table");
                var monthData = new List<MonthData>();

                for (int i = 5; i < ss.ChildNodes.Count - 1; i++)
                {
                    var mm = new MonthData
                    {
                        StockId = stock.StockId,
                        Name = stock.Name,
                        CreatedOn = DateTime.Now,
                        Datetime = Convert.ToDateTime(ss.ChildNodes[i].ChildNodes[0].InnerHtml + "/01"),
                        單月營收 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[1].InnerHtml.Replace(",", "")),
                        去年同月營收 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[2].InnerHtml.Replace(",", "")),
                        單月月增率 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[3].ChildNodes[0].InnerHtml.Replace("%", "")),
                        單月年增率 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[4].ChildNodes[0].InnerHtml.Replace("%", "")),
                        累計營收 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[5].InnerHtml.Replace(",", "")),
                        去年累計營收 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[6].InnerHtml.Replace(",", "")),
                        累積年增率 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[7].ChildNodes[0].InnerHtml.Replace("%", "")),
                    };

                    context.MonthData.Add(mm);
                }
               
                await context.SaveChangesAsync();

                Console.WriteLine($"{stock.StockId} {stock.Name} OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{stock.StockId} {stock.Name} {ex}");
            }
        }
    }
}
