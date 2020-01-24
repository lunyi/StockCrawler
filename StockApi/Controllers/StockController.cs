using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using DataService.DataModel;
using DataService.Models;
using DataService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StockApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        IStockQueries _stockQueries;

        public StockController(IStockQueries stockQueries)
        {
            _stockQueries = stockQueries;
        }

        [HttpGet, Route("{stockId}")]
        public Task<StockeModel> Get(string stockId)
        {
            return _stockQueries.GetPricesByStockIdAsync(stockId);
        }

        [HttpGet, Route("")]
        public Task<TwStock[]> Get()
        {
            return _stockQueries.GetTwStocksAsync();
        }
    }
}
