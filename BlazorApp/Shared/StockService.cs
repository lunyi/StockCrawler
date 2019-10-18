using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.JSInterop;

namespace BlazorApp.Shared
{
    public class StockService
    {
        [JSInvokable]
        public static async Task SetBestStockAsync(string stockId, string type, string desc)
        {
            IDataLayer dataLayer = new DataLayer();
            await dataLayer.SetBestStockAsync(stockId, type, desc);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetBestStocksAsync(int index)
        {
            IDataLayer dataLayer = new DataLayer();
            return await dataLayer.GetBestStocksAsync(index);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetStocksDateAsync(string datetime, int type)
        {
            IDataLayer dataLayer = new DataLayer();
            return await dataLayer.GetStocksByDateAsync(datetime, type);
        }

        [JSInvokable]
        public static async Task<string[]> GetDateListAsync()
        {
            IDataLayer dataLayer = new DataLayer();
            return await dataLayer.GetDaysAsync();
        }
    }
}
