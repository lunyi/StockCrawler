using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgresData.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class StockFutureParser : BaseParser
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi;
        private string _token;

        public StockFutureParser(LineNotifyBotApi lineNotifyBotApi)
        {
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public StockFutureParser()
        {
        }

        public async Task RunAsync()
        {
            var url = "https://www.taifex.com.tw/cht/2/stockLists";

            var context = new stockContext();

            var rootNode = GetRootNoteByUrl(url, true);

            var htmlNodes = rootNode.SelectNodes("//*[@id=\"myTable\"]/tbody/tr");

            var list = new List<BrokerTransactionDetail>();
            for (int j  = 0; j < htmlNodes.Count; j++)
            {
                var stockId = htmlNodes[j].ChildNodes[5].InnerText;
                var stock = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stockId);
                if (stock != null)
                {
                    stock.股票期貨 = true;
                    stock.Description = htmlNodes[j].ChildNodes[1].InnerText;
                }
            }
            await context.SaveChangesAsync();

            var temp = "";     
            var ss = context.Stocks.Where(p=>p.Status == 1 && p.股票期貨 == true && p.Industry != "金融保險業").Select(p=>p.Description).OrderBy(p => p).ToArray();
            for (int i = 0; i < ss.Length; i++)
            {
                temp += ss[i] + "FF0,";
            }
            var w = new StreamWriter("D:\\test.csv");
            await w.WriteLineAsync(temp);
            w.Close();
            await context.SaveChangesAsync();
        }

        private async Task NotifyBotApiAsync(stockContext context, string type)
        {
            var stocks = context.RealtimeBestStocks
                .Where(p => p.Datetime == DateTime.Today && p.Type == type)
                .OrderBy(p => p.StockId).ToArray();

            if (stocks.Any())
            {
                var s = new StringBuilder();
                s.AppendLine($@"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {type}");

                foreach (var stock in stocks)
                {
                    s.AppendLine($@"{stock.StockId} {stock.Name}");
                }

                await NotifyBotApiAsync(s.ToString());
            }
        }

        private async Task NotifyBotApiAsync(string message)
        {
            await _lineNotifyBotApi.Notify(new NotifyRequestDTO
            {
                AccessToken = _token,
                Message = message
            });
        }
    }
}
