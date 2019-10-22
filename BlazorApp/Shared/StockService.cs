using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using DataService.Services;
using Microsoft.JSInterop;

namespace BlazorApp.Shared
{
    public class StockService
    {
        [JSInvokable]
        public static async Task SetBestStockAsync(string stockId, string type)
        {
            IStockQueries dataLayer = new StockQueries();
            await dataLayer.SetBestStockAsync(stockId, type);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetBestStocksAsync(int index)
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetBestStocksAsync(index);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetStocksDateAsync(string datetime, int type)
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetStocksByDateAsync(datetime, type);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetStocksByTypeAsync(string type)
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetStocksByTypeAsync(type);
        }

        [JSInvokable]
        public static async Task<string[]> GetDateListAsync()
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetDaysAsync();
        }

        [JSInvokable]
        public static async Task<string[]> GetChosenStockTypesAsync()
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetChosenStockTypesAsync();
        }
    }
}
