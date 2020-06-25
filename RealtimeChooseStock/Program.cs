using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using SKCOMLib;

namespace RealtimeChooseStock
{
    class Program
    {
        static SKCenterLib PskCenter;
        static StockDbContext DbContext;
        static SKReplyLib m_pSKReply;
        static void Main(string[] args)
        {
            PskCenter = new SKCenterLib();
            m_pSKReply = new SKReplyLib();
            m_pSKReply.OnReplyMessage += OnAnnouncement;
            PskCenter.SKCenterLib_SetAuthority(1);
            var responseCode = PskCenter.SKCenterLib_Login("M121591178", "1q2w3e");

            if (responseCode != 0)
            {
                Console.WriteLine($"登入失敗 Response code : {responseCode}");
                return;
            }

            Console.WriteLine($"{DateTime.Now.TimeOfDay} 登入成功 {args[0]}");

            DbContext = new StockDbContext();

            Thread.Sleep(2000);
            GetMinuteKLines(int.Parse(args[0]), int.Parse(args[1]));
        }

        private static void GetMinuteKLines(int totalCount, int index)
        {
            var quote = new TwQuote(DbContext);
            var p = Stopwatch.StartNew();
            p.Start();

            var allStocks = DbContext.Stocks.Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToArray();

            var stocks = Split(allStocks, allStocks.Length / totalCount).ToArray();
            var tmpStocks = stocks.ToArray();
            var tmpStocks1 = tmpStocks[index].ToArray();
            var count = 0;
            //foreach (var stock in tmpStocks1)
            //{
            //    Console.WriteLine($"{DateTime.UtcNow} {count++} {stock.StockId} {stock.Name}");
            //    quote.GetPricesByStockId(stock.StockId, stock.Name);
            //}

            Parallel.ForEach(tmpStocks1, stock =>
            {
                Interlocked.Increment(ref count);
                Console.WriteLine($"{index}/{totalCount} {p.Elapsed.TotalSeconds} {count} {stock.StockId} {stock.Name}");
                quote.GetPricesByStockId(stock.StockId, stock.Name);
            });

            quote.SaveMinuteKLines();

            p.Stop();
            Console.WriteLine($"{p.Elapsed.TotalSeconds}");
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        static void OnAnnouncement(string strUserID, string bstrMessage, out short nConfirmCode)
        {
            Console.WriteLine(strUserID + "_" + bstrMessage);
            nConfirmCode = -1;
        }
    }
}
