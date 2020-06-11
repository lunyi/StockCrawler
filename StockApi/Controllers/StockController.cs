using System;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using DataService.DataModel;
using DataService.Models;
using DataService.Services;
using LineBotLibrary;
using Microsoft.AspNetCore.Mvc;

namespace StockApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockQueries _stockQueries;
        private readonly LineNotifyBotApi _lineNotifyBotApi;

        public StockController(IStockQueries stockQueries, LineNotifyBotApi lineNotifyBotApi)
        {
            _stockQueries = stockQueries;
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        [HttpGet, Route("{stockId}")]
        public Task<StockeModel> Get(string stockId, [FromQuery]DateTime datetime, bool chkDate)
        {
            return _stockQueries.GetPricesByStockIdAsync(stockId, datetime, chkDate);
        }

        [HttpGet, Route("")]
        public Task<TwStock[]> Get()
        {
            return _stockQueries.GetTwStocksAsync();
        }
    }
}
