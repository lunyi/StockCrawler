using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class UpdateStockListParser : BaseParser
    {
        public async Task RunAsync()
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new StockDbContext();
            var oldStockIds = context.Stocks
                .OrderBy(p => p.StockId)
                .Select(p => p.StockId)
                .ToList();

            var 上櫃股票 = 取得股票清單(4, "股票", "特別股");
            var 上市股票 = 取得股票清單(2, "股票", "上市認購");
            var newStocks = 上櫃股票.Union(上市股票);
            var newStockIds = newStocks
                .OrderBy(p => p.StockId)
                .Select(p => p.StockId.Trim())
                .ToArray();

            var stockIdsToRemove = oldStockIds.Except(newStockIds).ToArray();
            await RemoveStocksAsync(context, stockIdsToRemove);

            var stockIdsToAdd = newStockIds.Except(oldStockIds).ToArray();
            var stocksToCreate = newStocks.Where(p => stockIdsToAdd.Contains(p.StockId));
            await CreateStocksAsync(context, stocksToCreate.ToArray());

            var stockIdsToUpdate = oldStockIds.Except(stockIdsToRemove).ToArray();
            var stocksToUpdate = newStocks.Where(p => stockIdsToUpdate.Contains(p.StockId));
            await UpdateStocksAsync(context, stocksToUpdate.ToArray());

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
            Console.ReadLine();
        }

        private async Task UpdateStocksAsync(StockDbContext context, Stocks[] stocksToUpdate)
        {
            foreach (var stock in stocksToUpdate)
            {
                var stockToUpdate = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stock.StockId);

                if (stockToUpdate.Name != stock.Name ||
                    stockToUpdate.MarketCategory != stock.MarketCategory ||
                    stockToUpdate.Industry != stock.Industry)
                {

                    Console.WriteLine($"Update Stocks:{stockToUpdate.StockId} {stockToUpdate.Name}=>{stock.Name}");

                    var s1 = new StockHistory
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.StockId,
                        Name = stock.Name,
                        MarketCategory = stock.MarketCategory,
                        Industry = stock.Industry,
                        ListingOn = stock.ListingOn,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                    };

                    stockToUpdate.Name = stock.Name;
                    stockToUpdate.MarketCategory = stock.MarketCategory;
                    stockToUpdate.Industry = stock.Industry;
                    stockToUpdate.UpdatedOn = DateTime.Now;
                    context.StockHistory.Add(s1);
                    context.Entry<Stocks>(stockToUpdate).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task CreateStocksAsync(StockDbContext context, Stocks[] stockToAdd)
        {
            foreach (var stock in stockToAdd)
            {
                stock.Id = Guid.NewGuid();
                stock.Status = 1;

                Console.WriteLine($"Create Stocks:{stock.StockId} {stock.Name}");

                stock.CreatedOn = DateTime.Now;
                stock.UpdatedOn = DateTime.Now;
                context.Stocks.Add(stock);
            }
            await context.SaveChangesAsync();
        }


        private async Task RemoveStocksAsync(StockDbContext context, string[] stockIdsToRemove) 
        {   
            var stocksToRemove = await context.Stocks.Where(p => stockIdsToRemove.Contains(p.StockId)).ToArrayAsync();

            foreach (var stock in stocksToRemove)
            {
                stock.Status = 0;
                stock.UpdatedOn = DateTime.Now;
                Console.WriteLine($"Remove Stocks:{stock.StockId} {stock.Name}");
                context.Entry<Stocks>(stock).State = EntityState.Modified;
            }
            await context.SaveChangesAsync();
        }

        private Stocks[] 取得股票清單(int mode, string startKey, string endKey)
        {
            var url = $"https://isin.twse.com.tw/isin/C_public.jsp?strMode={mode}";
            var rootNode = GetRootNoteByUrl(url, false);
            var n1 = rootNode.ChildNodes[3].ChildNodes[1];

            var s = new List<Stocks>();

            var start = false;

            for (int i = 0; i < n1.ChildNodes.Count; i++)
            {
                var tr = n1.ChildNodes[i];
                if (tr.ChildNodes.Count >= 4 && start)
                {
                    var tmp = tr.ChildNodes[0].InnerText.Split('　');
                    s.Add(new Stocks
                    {
                        StockId = tmp[0],
                        Name = tmp[1],
                        ListingOn = Convert.ToDateTime(tr.ChildNodes[2].InnerText),
                        MarketCategory = tr.ChildNodes[3].InnerText,
                        Industry = tr.ChildNodes[4].InnerText
                    });
                }

                if (tr.InnerText.Contains(startKey))
                {
                    start = true;
                    continue;
                }

                if (tr.InnerText.Contains(endKey))
                {
                    start = false;
                    break;
                }
            }

            return s.ToArray();
        }
    }
}
