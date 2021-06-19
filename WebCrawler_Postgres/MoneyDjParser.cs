using PostgresData.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class MoneyDjParser : BaseParser
    {
        public async Task RunAsync()
        {
            var context = new stockContext();

            var stocks = context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToList();

            var s = Stopwatch.StartNew();
            s.Start();

            foreach (var stock in stocks)
            {
                await ParserStockAsync(context, stock);
            }

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
            Console.ReadLine();
        }

        private async Task ParserStockAsync(stockContext context, PostgresData.Models.Stock stock)
        {
            try 
            {
                var rootNode = GetRootNoteByUrl($"http://5850web.moneydj.com/z/zc/zca/zca_{stock.StockId}.djhtm", false);

                Console.WriteLine($"Parser {stock.StockId} {stock.Name}");
                var tmp股本 = Convert.ToDecimal(rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[14]/td[2]").InnerText);
                var tmp營收比重 = rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[21]/td[2]").InnerText;
                var tmpWebsite = rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[23]/td[2]").InnerText;
                var tmpAddress = rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[24]/td[2]").InnerText;

                if (stock.股本 != tmp股本 || stock.營收比重 != tmp營收比重 || stock.Website!= tmpWebsite || stock.Address != tmpAddress)
                {
                    var s1 = new StockHistory
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.StockId,
                        Name = stock.Name,
                        MarketCategory = stock.MarketCategory,
                        Industry = stock.Industry,
                        ListingOn = stock.ListingOn,
                        股本 = stock.股本,
                        營收比重 = stock.營收比重,
                        Website = stock.Website,
                        Address = stock.Address,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                    };

                    stock.股本 = Convert.ToDecimal(rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[14]/td[2]").InnerText);
                    stock.營收比重 = rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[21]/td[2]").InnerText;
                    stock.Website = rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[23]/td[2]").InnerText;
                    stock.Address = rootNode.SelectSingleNode("/html/body/div/table/tr[2]/td[2]/table/tr[1]/td/table/tr[24]/td[2]").InnerText;

                    Console.WriteLine($"Update {stock.StockId} {stock.Name}");
                    context.StockHistories.Add(s1);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{stock.StockId} Failed");
            }
        }
    }
}
