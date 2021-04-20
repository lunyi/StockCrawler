using System;
using System.Threading.Tasks;
using DataService.DataModel;
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
        public static async Task RemoveBestStockAsync(string stockId, string type)
        {
            IStockCommands commands = new StockCommands();
            await commands.RemoveBestStockAsync(stockId, type);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetBestStocksAsync(int index)
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetBestStocksAsync(index);
        }

        [JSInvokable]
        public static Task<BrokerInfo[]> GetBrokersAsync(string stock, string date, int days)
        {
            IStockQueries dataLayer = new StockQueries();
            return dataLayer.GetBrokersAsync(stock, date, days);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetStocksDateAsync(string datetime, int type)
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetStocksByDateAsync(datetime, type);
        }

        [JSInvokable]
        public static async Task<Stocks[]> GetStocksByTypeAsync(string type, string datetime)
        {
            IStockQueries dataLayer = new StockQueries();
            return await dataLayer.GetStocksByTypeAsync(type, datetime);
        }

        [JSInvokable]
        public static Task<BestStockType[]> GetBestStockTypeAsync()
        {
            IStockQueries dataLayer = new StockQueries();
            return dataLayer.GetBestStockTypeAsync();
        }

        [JSInvokable]
        public static Task<Stocks[]> GetStocksByBestStockTypeAsync(string name, string datetime)
        {
            IStockQueries dataLayer = new StockQueries();
            return dataLayer.GetStocksByBestStockTypeAsync(name, datetime);
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

        [JSInvokable]
        public static Task<StockeModel> GetPricesByStockIdAsync(string stockId, DateTime datetime, bool chkDate)
        {
            IStockQueries dataLayer = new StockQueries();
            return dataLayer.GetPricesByStockIdAsync(stockId, datetime, chkDate);
        }
    }
}
