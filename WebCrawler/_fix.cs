using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class _fixParser : BaseParser
    {
        //https://jdata.yuanta.com.tw/z/zc/zco/zco0/zco0.djhtm?A=2330&BHID=1480&b=1480&C=1&D=2019-6-1&E=2020-7-22&ver=V3
        //https://pscnetsecrwd.moneydj.com/b2brwdCommon/jsondata/ef/3f/9b/twStockData.xdjjson?x=stock-chip0002-7&d=2019/03/24&e=2020/07/23&a=1229&b=1520&revision=2018_07_31_1
        //string baseUrl = "https://fubon-ebrokerdj.fbs.com.tw";
        string baseUrl = "http://5850web.moneydj.com";
        //string baseUrl = "https://concords.moneydj.com";
        private string _token;

        public ConcurrentDictionary<string, string> ErrorStocks { get; set; }

        [Obsolete]
        public async Task RunAsync(DateTime date)
        {
            //ExecuteByStock("2642", "2642");
            var context = new StockDbContext();
            var s = Stopwatch.StartNew();
            s.Start();

            var oldPrices = await context.Prices
              .Where(p => p.Datetime == date)
              .ToArrayAsync();

            for (int i = 0; i < oldPrices.Length; i++)
            {
                try
                {
                    ParseMargin(oldPrices[i].StockId, oldPrices[i]);
                    Console.WriteLine($"{i}/{oldPrices.Length}   {oldPrices[i].StockId} {oldPrices[i].Name}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{ oldPrices[i].StockId} { oldPrices[i].Name} : Error {e}");
                }
            }

            context.Database.SetCommandTimeout(300);
            context.SaveChanges();
            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
        }

        private void ParseMargin(string stockId, Price price)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var rootNode = GetRootNoteByUrl($"{baseUrl}/z/zc/zcn/zcn_{stockId}.djhtm", false);
            var htmlNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/form/table/tr/td/table/tr[8]");

            if (htmlNode == null)
                return;

            var dateArray = htmlNode.ChildNodes[1].InnerHtml.Split(new[] { '/' });

            if (dateArray.Length != 3)
                return;

            var date = new DateTime(Convert.ToInt32(dateArray[0]) + 1911, Convert.ToInt32(dateArray[1]), Convert.ToInt32(dateArray[2]));
            if (date == price.Datetime)
            {
                price.融資買進 = Convert.ToInt32(htmlNode.ChildNodes[3].InnerText.Replace(",", ""));
                price.融資賣出 = Convert.ToInt32(htmlNode.ChildNodes[5].InnerText.Replace(",", ""));
                price.融資現償 = Convert.ToInt32(htmlNode.ChildNodes[7].InnerText.Replace(",", ""));
                price.融資餘額 = Convert.ToInt32(htmlNode.ChildNodes[9].InnerText.Replace(",", ""));
                var used = htmlNode.ChildNodes[15].InnerText.Replace("%", "");
                price.融資使用率 = used == "" ? 0 : Convert.ToDecimal(used);
                price.融券賣出 = Convert.ToInt32(htmlNode.ChildNodes[17].InnerText.Replace(",", ""));
                price.融券買進 = Convert.ToInt32(htmlNode.ChildNodes[19].InnerText.Replace(",", ""));
                price.融券餘額 = Convert.ToInt32(htmlNode.ChildNodes[23].InnerText.Replace(",", ""));
                price.資券相抵 = Convert.ToInt32(htmlNode.ChildNodes[29].InnerText.Replace(",", ""));
            }

            s.Stop();
            Console.WriteLine("融資：" + s.Elapsed.TotalSeconds);
        }
    }
}
