using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using DataService.DataModel;
using DataService.Models;
using DataService.Services;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<StockeModel> Get(string stockId)
        {
            var token = await _stockQueries.GetTokenAsync();
            var res = await _stockQueries.GetPricesByStockIdAsync(stockId);

            await _lineNotifyBotApi.Notify(new NotifyRequestDTO
            {
                AccessToken = token,
                Message = $"{stockId} {res.Stock.Name}"
            });

            return res;
        }

        [HttpGet, Route("")]
        public Task<TwStock[]> Get()
        {
            return _stockQueries.GetTwStocksAsync();
        }
    }
}
