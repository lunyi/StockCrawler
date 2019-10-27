using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class ChipParser : BaseParser
    {
        public async Task RunAsync()
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new StockDbContext();
            //var stocks = context.Stocks.Where(p => p.Status == 1).ToArray();
            var stocks = context.Stocks.FromSqlRaw(SQL()).ToArray();
            var count = stocks.Count();
            var startDate = "2019-08-01";
            var endDate = "2019-09-30";
            var objStartDate = Convert.ToDateTime(startDate);
            var objEndDate = Convert.ToDateTime(endDate);
            var chips = new ConcurrentBag<Chip>();
            decimal 買進 = 0, 賣出 = 0;
            try
            {
                Parallel.ForEach(stocks, async (stock, state, index) =>
                {
                    try
                    {
                        Console.WriteLine($"{index}/{count} {stock.StockId}, Thread ID={Thread.CurrentThread.ManagedThreadId}");
                        (買進, 賣出) = ParseMainForce(stock.StockId, startDate, endDate);
                        await Task.Delay(200);

                        var chip = new Chip
                        {
                            Id = Guid.NewGuid(),
                            StockId = stock.StockId,
                            Name = stock.Name,
                            StartDate = objStartDate,
                            EndDate = objEndDate,
                            主力買進 = 買進,
                            主力賣出 = 賣出,
                            CreatedOn = DateTime.Now
                        };
                        chips.Add(chip);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"{index}/{count} {stock.StockId} Failed");
                        state.Stop();
                    }
                });
            }
            finally
            {
                Console.WriteLine($"Start Save");
                context.BulkInsert(chips.ToList());
                Console.WriteLine($"OK!!!");
                s.Stop();
                Console.WriteLine($"{s.Elapsed.TotalSeconds} sec");
            }
        }

        public (decimal, decimal) ParseMainForce(string stockId, string startDate, string endDate)
        {
            var mainForces = new ConcurrentDictionary<int, Tuple<decimal, decimal>>();
            var url = $"https://concords.moneydj.com/z/zc/zco/zco.djhtm?a={stockId}&e={startDate}&f={endDate}";
            var rootNode = GetRootNoteByUrl(url, false);
            var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

            decimal 主力買超張數 = 0, 主力賣超張數 = 0;

            for (int i = 6; i < nodes.Count; i++)
            {
                var node = nodes[i];

                if (node.ChildNodes[1].InnerHtml == "合計買超張數")
                {
                    主力買超張數 = Convert.ToDecimal(node.ChildNodes[3].InnerHtml.Replace(",", ""));
                    主力賣超張數 = Convert.ToDecimal(node.ChildNodes[7].InnerHtml.Replace(",", ""));

                }
                else if (node.ChildNodes[1].InnerHtml == "合計買超股數")
                {
                    主力買超張數 = Convert.ToDecimal(node.ChildNodes[3].InnerHtml.Replace(",", "")) / 1000;
                    主力賣超張數 = Convert.ToDecimal(node.ChildNodes[7].InnerHtml.Replace(",", "")) / 1000;
                }
            }

            return (主力買超張數, 主力賣超張數);
        }

        private string SQL() 
        { 
            return $@"
select stock.*
from [Stocks] stock 
left join (select * from [Chip] where StartDate = '2019-08-01') chip on stock.StockId  = chip.StockId
where stock.Status = 1 and chip.Id is null
order by stock.StockId
";
        }
    }
}
