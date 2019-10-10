using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Shared
{
    public interface IDataLayer
    {
        Task<Stocks[]> GetStocksAsync();
    }
    public class DataLayer : IDataLayer
    {
        string[] NotContainStocks = new string[] {
                "0050",
                "0051",
                "0052",
                "0053",
                "0054",
                "0055",
                "0056",
                "0057",
                "0058",
                "0059",
                "0060",
                "0061"
            };

        Task<Stocks[]> IDataLayer.GetStocksAsync()
        {
            var context = new StockDbContext();

            return context.Stocks
                .Where(p => !NotContainStocks.Contains(p.StockId))
                .OrderBy(p=>p.StockId)
                .ToArrayAsync();
        }
    }
}
