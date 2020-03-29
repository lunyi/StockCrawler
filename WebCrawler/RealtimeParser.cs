﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class RealtimeParser : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string Token;

        public RealtimeParser(LineNotifyBotApi lineNotifyBotApi)
        { 
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public async Task RunAsync()
        {
            var url = $"https://histock.tw/app/table.aspx";
            var rootNode = GetRootNoteByUrl(url);
            var nodes = rootNode.SelectNodes("//*[@id='fm']/div[4]/div[3]/div[1]/div/div/table/tr");

            var s = new Dictionary<string, Stock[]>();

            for (int i = 1; i < nodes.Count; i++)
            {
                var key = nodes[i].SelectSingleNode("th").InnerText;
                var stocks = nodes[i].SelectNodes("td/a");
                if (stocks == null) continue;

                var stockList = new List<Stock>();

                for (int j = 0; j < stocks.Count; j++)
                {
                    var name = stocks[j].InnerText;
                    var stockid = stocks[j].Attributes[0].Value.Substring(7);
                    stockList.Add(new Stock(stockid, name));
                }

                s.Add(key, stockList.ToArray());
            }

            await InsertAsync(s);
        }

        private async Task InsertAsync(Dictionary<string, Stock[]> newStocks)
        {
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1);
            var currentBests = context.RealtimeBestStocks.Where(p => p.Datetime == DateTime.Today);

            foreach (var item in newStocks)
            {
                var stockids = item.Value.Select(p=>p.StockId).ToArray();
                var stockList = stocks.Where(s => stockids.Contains(s.StockId)).ToArray();

                foreach (var stock in stockList)
                {
                    var newBest = currentBests.FirstOrDefault(p=>p.StockId == stock.StockId && p.Type == item.Key);
                    if (newBest == null)
                    {
                        var realtime = new RealtimeBestStocks
                        {
                            Id = Guid.NewGuid(),
                            StockId = stock.StockId,
                            Name = stock.Name,
                            Type = item.Key,
                            Datetime = DateTime.Today
                        };

                        Console.WriteLine($"{item.Key} {stock.StockId}");
                        context.RealtimeBestStocks.Add(realtime);
                    }
                }           
            }

            ParseWarnStock(context, currentBests);
            await context.SaveChangesAsync();

            Token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
            await NotifyBotApiAsync(context, "突破整理區間");
            await NotifyBotApiAsync(context, "多頭吞噬");
            await NotifyBotApiAsync(context, "突破季線");
        }

        private async Task NotifyBotApiAsync(StockDbContext context, string type)
        {
            var stocks = context.RealtimeBestStocks
                .Where(p => p.Datetime == DateTime.Today && p.Type == type)
                .OrderBy(p=>p.StockId).ToArray();

            if (stocks.Any())
            {
                var s = new StringBuilder();
                s.AppendLine($@"{DateTime.Now.ToString("yyyy-MM-dd HH:ms:ss")} {type}");

                foreach (var stock in stocks)
                {
                    s.AppendLine($@"{stock.StockId} {stock.Name}");
                }

                await NotifyBotApiAsync(s.ToString());
            }
        }

        private async Task NotifyBotApiAsync(string message)
        {
            await _lineNotifyBotApi.Notify(new NotifyRequestDTO
            {
                AccessToken = Token,
                Message = message
            });
        }

        private void ParseWarnStock(StockDbContext context, IQueryable<RealtimeBestStocks> current)
        {
            string type = "警示股";
            var url = $"http://5850web.moneydj.com/z/ze/zew/zew.djhtm";
            var rootNode = GetRootNoteByUrl(url, false);
            var nodes = rootNode.SelectNodes("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr");

            var list = new List<RealtimeBestStocks>();

            for (int i = 3; i < nodes.Count; i++)
            {
                var sIndex = nodes[i].ChildNodes[3].InnerHtml.IndexOf("'AS");

                if (sIndex > 0)
                {
                    var stockId = nodes[i].ChildNodes[3].InnerHtml.Substring(sIndex + 3, 4);
                    var temp = nodes[i].ChildNodes[3].InnerHtml.Substring(sIndex + 10, 8);
                    var name = temp.Substring(0, temp.IndexOf("'"));
                    var newBest = current.FirstOrDefault(p => p.StockId == stockId && p.Type == type);

                    if (newBest == null)
                    {
                        list.Add(new RealtimeBestStocks
                        {
                            Id = Guid.NewGuid(),
                            StockId = stockId,
                            Name = name,
                            Type = type,
                            Datetime = DateTime.Today
                        });
                    }
                }
            }

            context.RealtimeBestStocks.AddRange(list);
        }
    }

    public class Stock 
    {
        public Stock(string stockid, string name)
        {
            StockId = stockid;
            Name = name;
        }
        public string StockId { get; set; }
        public string Name { get; set; }
    }
}
