using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface IStockCommands
    {
        Task CreatePriceAsync(Prices price);
        Task UpdateStockAsync(Stocks stock);
    }
    public class StockCommands : IStockCommands
    {
        async Task IStockCommands.CreatePriceAsync(Prices price)
        {
            var context = new StockDbContext();
            var p = context.Prices.FirstOrDefault(p => p.Datetime == price.Datetime && p.StockId == price.StockId);
            if (p == null)
            {
                context.Prices.Add(price);
            }

            await context.SaveChangesAsync();
        }

        async Task IStockCommands.UpdateStockAsync(Stocks stock)
        {
            var context = new StockDbContext();
            var p = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stock.StockId);
            if (p != null)
            {
                context.Entry<Stocks>(p).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }
    }
}
