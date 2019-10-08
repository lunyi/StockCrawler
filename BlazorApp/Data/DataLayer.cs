using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;

namespace BlazorApp.Data
{
    public interface IDataLayer
    {
        Stocks[] GetStocks();
    }
    public class DataLayer : IDataLayer
    {
        Stocks[] IDataLayer.GetStocks()
        {
            var context = new StockDbContext();
            return context.Stocks.ToArray();
        }
    }
}
