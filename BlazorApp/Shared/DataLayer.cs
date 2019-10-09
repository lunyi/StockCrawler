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
        Task<Stocks[]> IDataLayer.GetStocksAsync()
        {
            var context = new StockDbContext();
            return context.Stocks.OrderBy(p=>p.StockId).ToArrayAsync();
        }
    }
}
