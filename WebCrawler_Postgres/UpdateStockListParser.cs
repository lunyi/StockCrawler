using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PostgresData.Models;
using Microsoft.EntityFrameworkCore;
using Stock = PostgresData.Models.Stock;

namespace WebCrawler
{
    public class UpdateStockListParser : BaseParser
    {
        public async Task RunAsync()
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new stockContext();
            var oldStockIds = context.Stocks
                .OrderBy(p => p.StockId)
                .Select(p => p.StockId)
                .ToList();

            var 上櫃股票 = 取得股票清單(4, "股票", "特別股");
            var 上市股票 = 取得股票清單(2, "股票", "上市認購");
            var newStock = 上櫃股票.Union(上市股票);
            var newStockIds = newStock
                .OrderBy(p => p.StockId)
                .Select(p => p.StockId.Trim())
                .ToArray();

            var stockIdsToRemove = oldStockIds.Except(newStockIds).ToArray();
            await RemoveStockAsync(context, stockIdsToRemove);

            var stockIdsToAdd = newStockIds.Except(oldStockIds).ToArray();
            var stocksToCreate = newStock.Where(p => stockIdsToAdd.Contains(p.StockId));
            await CreateStockAsync(context, stocksToCreate.ToArray());

            var stockIdsToUpdate = oldStockIds.Except(stockIdsToRemove).ToArray();
            var stocksToUpdate = newStock.Where(p => stockIdsToUpdate.Contains(p.StockId));
            await UpdateStockAsync(context, stocksToUpdate.ToArray());

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
            Console.ReadLine();
        }

        private async Task UpdateStockAsync(stockContext context, PostgresData.Models.Stock[] stocksToUpdate)
        {
            foreach (var stock in stocksToUpdate)
            {
                var stockToUpdate = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stock.StockId);

                if (stockToUpdate.Name != stock.Name ||
                    stockToUpdate.MarketCategory != stock.MarketCategory ||
                    stockToUpdate.Industry != stock.Industry)
                {

                    Console.WriteLine($"Update Stock:{stockToUpdate.StockId} {stockToUpdate.Name}=>{stock.Name}");

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
                    context.StockHistories.Add(s1);
                    context.Entry<PostgresData.Models.Stock>(stockToUpdate).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task CreateStockAsync(stockContext context, PostgresData.Models.Stock[] stockToAdd)
        {
            foreach (var stock in stockToAdd)
            {
                stock.Status = 1;

                Console.WriteLine($"Create Stock:{stock.StockId} {stock.Name}");

                stock.CreatedOn = DateTime.Now;
                stock.UpdatedOn = DateTime.Now;
                context.Stocks.Add(stock);
            }
            await context.SaveChangesAsync();
        }


        private async Task RemoveStockAsync(stockContext context, string[] stockIdsToRemove) 
        {   
            var stocksToRemove = await context.Stocks.Where(p => stockIdsToRemove.Contains(p.StockId)).ToArrayAsync();

            foreach (var stock in stocksToRemove)
            {
                stock.Status = 0;
                stock.UpdatedOn = DateTime.Now;
                Console.WriteLine($"Remove Stock:{stock.StockId} {stock.Name}");
                context.Entry<PostgresData.Models.Stock>(stock).State = EntityState.Modified;
            }
            await context.SaveChangesAsync();
        }

        private PostgresData.Models.Stock[] 取得股票清單(int mode, string startKey, string endKey)
        {
            var url = $"https://isin.twse.com.tw/isin/C_public.jsp?strMode={mode}";
            var rootNode = GetRootNoteByUrl(url, false);
            var n1 = rootNode.ChildNodes[3].ChildNodes[1];

            var s = new List<PostgresData.Models.Stock>();

            var start = false;

            for (int i = 0; i < n1.ChildNodes.Count; i++)
            {
                var tr = n1.ChildNodes[i];
                if (tr.ChildNodes.Count >= 4 && start)
                {
                    var tmp = tr.ChildNodes[0].InnerText.Split('　');
                    s.Add(new PostgresData.Models.Stock
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
