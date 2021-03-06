﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface IStockCommands
    {
        Task CreatePriceAsync(Price price);
        Task UpdateStockAsync(Stock stock);
        Task RemoveBestStockAsync(string stockId, string type);
    }
    public class StockCommands : IStockCommands
    {
        async Task IStockCommands.CreatePriceAsync(Price price)
        {
            var context = new StockDbContext();
            var p = context.Prices.FirstOrDefault(p => p.Datetime == price.Datetime && p.StockId == price.StockId);
            if (p == null)
            {
                context.Prices.Add(price);
            }

            await context.SaveChangesAsync();
        }

       async  Task IStockCommands.RemoveBestStockAsync(string stockId, string type)
        {
            var context = new StockDbContext();
            var stock = await context.BestStocks.FirstOrDefaultAsync(p => p.StockId == stockId && p.Type == type);

            if (stock != null)
            {
                context.Entry(stock).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        async Task IStockCommands.UpdateStockAsync(Stock stock)
        {
            var context = new StockDbContext();
            var p = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stock.StockId);
            if (p != null)
            {
                context.Entry(p).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }
    }
}
