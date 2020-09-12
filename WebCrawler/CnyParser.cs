using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class CnyParser : BaseParser
    {
        //https://jdata.yuanta.com.tw/z/zc/zco/zco0/zco0.djhtm?A=2330&BHID=1480&b=1480&C=1&D=2019-6-1&E=2020-7-22&ver=V3
        //https://pscnetsecrwd.moneydj.com/b2brwdCommon/jsondata/ef/3f/9b/twStockData.xdjjson?x=stock-chip0002-7&d=2019/03/24&e=2020/07/23&a=1229&b=1520&revision=2018_07_31_1
        //string baseUrl = "https://fubon-ebrokerdj.fbs.com.tw";
        string baseUrl = "http://5850web.moneydj.com";
        //string baseUrl = "https://concords.moneydj.com";
        private string _token;

        public ConcurrentDictionary<string, string> ErrorStocks { get; set; }

        [Obsolete]
        public async Task RunAsync(int index, int partition)
        {
            //ExecuteByStock("2642", "2642");
            var context = new StockDbContext();
            var s = Stopwatch.StartNew();
            s.Start();

            var stocks = await context.Stocks
                .Where(p => p.Status == 1)
                .OrderBy(p => p.StockId)
                .ToArrayAsync();

            var oldPrices = await context.Prices
              .Where(p => p.Datetime == DateTime.Today)
              .ToArrayAsync();

            int start = (index - 1) * stocks.Length / partition;
            int end = index * stocks.Length / partition;

            var prices = new List<Prices>();
            var seq = 0;
            for (int i = start; i < end; i++)
            {
                try
                {
                    var oldPrice = oldPrices.FirstOrDefault(p => p.StockId == stocks[i].StockId);
                    var price = ExecuteByStock(oldPrice, stocks[i].StockId, stocks[i].Name);
                    prices.Add(price);

                    Console.WriteLine($"{index}/{partition}  {seq++}/{end - start}  {stocks[i].StockId} {stocks[i].Name}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{ stocks[i].StockId} { stocks[i].Name} : Error {e}");
                }
            }

            context.Database.SetCommandTimeout(180);

            prices = prices.Where(p => p != null).ToList();
            try 
            {
                await context.BulkInsertOrUpdateAsync(prices);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{index}/{partition}");
                Console.WriteLine($"Error {e}");

            }

            context.Database.ExecuteSqlCommand($"exec [usp_Update_MA_And_VMA] {DateTime.Today:yyyy-MM-dd}");
            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
        }

        private Prices ExecuteByStock(Prices oldPrice, string stockid, string name)
        {
            if (oldPrice != null)
            {
                return ParserLastDay(oldPrice, stockid, name);
            }
                
            var price = ParseSingleHistoryPrice(stockid, name);
            if (price != null)
            {
                return ParserLastDay(price, stockid, name);
            }
            return null;
        }

        private async Task NotifyAsync(StockDbContext context)
        {
            _token = await context.Token.Select(p => p.LineToken).FirstOrDefaultAsync();
        }

        private static string GetSql()
        {
            return @$"
  select s.* from [Stocks]  s 
  left join (select * from [Prices] where [Datetime] = '{DateTime.Today:yyyy/MM/dd}') p on s.StockId = p.StockId
  where  s.Status = 1 and p.Id is null
  order by s.StockId";
        }

        [Obsolete]
        public async Task RunMainForceAsync()
        {
            var context = new StockDbContext();
            var s = Stopwatch.StartNew();
            s.Start();

            var parser = new CnyParser();
            var stocks = context.Stocks.Where(p=>p.Status==1).OrderByDescending(p=>p.StockId).ToList();

            foreach (var item in stocks)
            {
                for (int i = 0; i < 120; i++)
                {
                    var datetime = DateTime.Now.AddDays(i * -1 - 1).ToString("yyyy-MM-dd");
                    try
                    {
                        await ParseMainForce(context, item.StockId, item.Name, datetime);
                    }
                    catch (Exception ex)
                    { 
                        Log($"{item.StockId}, {item.Name}, {datetime}, {ex.Message}");
                    }
                }
            }

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
        }

        public Prices ParserLastDay(Prices price, string stockId, string name)
        {
            try
            {
                ParseMargin(stockId, price);
                ParseInst(stockId, price);
                ParseMainForce(stockId, DateTime.Today.ToString("yyyy/MM/dd"), price);
                ParseTrust(stockId, price);
                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{stockId} : Failed: {ex.Message}");
                Console.WriteLine($"{stockId} : Failed: {ex.StackTrace}");
                ErrorStocks.AddOrUpdate(stockId, name, (key, oldValue) => name);
                return null;
            }
        }

        private void ParseMargin(string stockId, Prices price)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var rootNode = GetRootNoteByUrl($"{baseUrl}/z/zc/zcn/zcn_{stockId}.djhtm", false);
            var htmlNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/form/table/tr/td/table/tr[8]");

            if (htmlNode == null)
                return;

            var dateArray = htmlNode.ChildNodes[1].InnerHtml.Split(new[] {'/'});

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

        private void ParseInst(string stockId, Prices price)
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var url = $"{baseUrl}/z/zc/zcl/zcl_{stockId}.djhtm";
            var rootNode = GetRootNoteByUrl(url, false);
            var htmlNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/form/table/tr/td/table/tr[8]");
            if (htmlNode == null)
            {
                htmlNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/form/table/tr/td/table/tr[8]");                                        
            }

            var dateArray = htmlNode.ChildNodes[1].InnerHtml.Split(new[] { '/' });
            var date = new DateTime(Convert.ToInt32(dateArray[0]) + 1911, Convert.ToInt32(dateArray[1]), Convert.ToInt32(dateArray[2]));

            if (date == price.Datetime)
            {
                price.外資買賣超 = htmlNode.ChildNodes[3].InnerText.Trim() == "--" ? 0 : Convert.ToInt32(htmlNode.ChildNodes[3].InnerText.Replace(",", ""));
                price.投信買賣超 = htmlNode.ChildNodes[5].InnerText.Trim() == "--" ? 0 : Convert.ToInt32(htmlNode.ChildNodes[5].InnerText.Replace(",", ""));
                price.自營商買賣超 = htmlNode.ChildNodes[7].InnerText.Trim() == "--" ? 0 : Convert.ToInt32(htmlNode.ChildNodes[7].InnerText.Replace(",", ""));
                price.外資持股 = htmlNode.ChildNodes[11].InnerText.Trim() == "--" ? 0 : Convert.ToInt32(htmlNode.ChildNodes[11].InnerText.Replace(",", ""));
                price.投信持股 = htmlNode.ChildNodes[13].InnerText.Trim() == "--" ? 0 : Convert.ToInt32(htmlNode.ChildNodes[13].InnerText.Replace(",", ""));
                price.自營商持股 = htmlNode.ChildNodes[15].InnerText.Trim() == "--" ? 0 : Convert.ToInt32(htmlNode.ChildNodes[15].InnerText.Replace(",", ""));
                price.外資持股比例 = htmlNode.ChildNodes[19].InnerText.Trim() == "--" ? 0 : Convert.ToDecimal(htmlNode.ChildNodes[19].InnerText.Replace("%", ""));
            }

            s.Stop();
            Console.WriteLine("外資：" + s.Elapsed.TotalSeconds);
        }

      

        public CnyParser()
        {
            ErrorStocks = new ConcurrentDictionary<string, string>();
        }

        public void ParseTrust(string stockId, Prices price) 
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var datetime = DateTime.Now.ToString("yyyy-MM-dd");
            var url = $@"{baseUrl}/z/zc/zcj/zcj_{stockId}.djhtm";

            var rootNode = GetRootNoteByUrl(url, false);

            var instPercent = rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[5]/td[4]").InnerHtml;
            price.董監持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[3]/td[2]").InnerHtml.Replace(",", ""));

            try
            {
                price.董監持股比例 = Convert.ToDecimal(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[3]/td[4]").InnerHtml.Replace(",", "").Replace("%", ""));
            }
            catch { 
            
            }
           
            price.董監持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[3]/td[2]").InnerHtml.Replace(",", ""));
            price.外資持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[4]/td[2]").InnerHtml.Replace(",", ""));
            price.投信持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[5]/td[2]").InnerHtml.Replace(",", ""));
            price.投信持股比例 = instPercent == "" ? 0 : Convert.ToDecimal(instPercent.Replace(",", "").Replace("%", ""));
            price.自營商持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[6]/td[2]").InnerHtml.Replace(",", ""));


            s.Stop();
            Console.WriteLine("持股：" + s.Elapsed.TotalSeconds);
        }

        public void ParseMainForce(string stockId,string datetime, Prices price)
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var mainForces = new ConcurrentDictionary<int, Tuple<decimal, decimal>>();

            for (int index = 1; index <= 6; index++)
            {
                var url = $"{baseUrl}/z/zc/zco/zco_{stockId}_{index}.djhtm";
                var rootNode = GetRootNoteByUrl(url, false);
                var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

                decimal 主力買超張數 = 0, 主力賣超張數 = 0;

                for (int i = 6; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    if (node.ChildNodes.Count>1)
                    {
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
                }
                mainForces.TryAdd(index, Tuple.Create(主力買超張數, 主力賣超張數));
            }

            price.主力買超張數 = mainForces[1].Item1;
            price.主力賣超張數 = mainForces[1].Item2;
            price.五日主力買超張數 = mainForces[2].Item1;
            price.五日主力賣超張數 = mainForces[2].Item2;
            price.十日主力買超張數 = mainForces[3].Item1;
            price.十日主力賣超張數 = mainForces[3].Item2;
            price.二十日主力買超張數 = mainForces[4].Item1;
            price.二十日主力賣超張數 = mainForces[4].Item2;
            price.四十日主力買超張數 = mainForces[5].Item1;
            price.四十日主力賣超張數 = mainForces[5].Item2;
            price.六十日主力買超張數 = mainForces[6].Item1;
            price.六十日主力賣超張數 = mainForces[6].Item2;

            s.Stop();
            Console.WriteLine("主力買賣超：" + s.Elapsed.TotalSeconds);
        }

        private async Task ParseMainForce(StockDbContext context, string stockId, string name, string datetime)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var url = $"{baseUrl}/z/zc/zco/zco.djhtm?a={stockId}&e={datetime}&f={datetime}";
            var rootNode = GetRootNoteByUrl(url, false);
            var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

            if (nodes.Count < 8)
            {
                return;
            }
            var divide = 1;
            if (nodes[^3].ChildNodes[1].InnerHtml == "合計買超股數")
            {
                divide = 1000;
            }
            var list = new List<BrokerTransaction>();

            var datetime1 = Convert.ToDateTime(datetime);
            for (int i = 6; i <= nodes.Count - 4; i++)
            {
                var node = nodes[i];
                var brokerName = node.ChildNodes[1].ChildNodes[0].InnerHtml;
                var broker = await context.BrokerTransaction.FirstOrDefaultAsync(p => p.Datetime == datetime1 && p.StockId == stockId && p.BrokerName == brokerName);
                if (brokerName != "&nbsp;" && broker != null)
                {
                    var brokerBuy = new BrokerTransaction();
                    brokerBuy.Id = Guid.NewGuid();
                    brokerBuy.StockId = stockId;
                    brokerBuy.Name = name;
                    brokerBuy.Datetime = datetime1;
                    brokerBuy.BrokerName = node.ChildNodes[1].ChildNodes[0].InnerHtml;
                    brokerBuy.Buy = Convert.ToDecimal(node.ChildNodes[3].InnerHtml.Replace(",", "")) / divide;
                    brokerBuy.Sell = Convert.ToDecimal(node.ChildNodes[5].InnerHtml.Replace(",", "")) / divide;
                    brokerBuy.買賣超 = Convert.ToDecimal(node.ChildNodes[7].InnerHtml.Replace(",", "")) / divide;
                    brokerBuy.Percent = Convert.ToDecimal(node.ChildNodes[9].InnerHtml.Replace("%", ""));
                    list.Add(brokerBuy);
                }

                brokerName = node.ChildNodes[11].ChildNodes[0].InnerHtml;
                broker = await context.BrokerTransaction.FirstOrDefaultAsync(p => p.Datetime == datetime1 && p.StockId == stockId && p.BrokerName == brokerName);

                if (brokerName != "&nbsp;" && broker != null)
                {
                    var brokerSell = new BrokerTransaction();
                    brokerSell.Id = Guid.NewGuid();
                    brokerSell.StockId = stockId;
                    brokerSell.Name = name;
                    brokerSell.Datetime = datetime1;
                    brokerSell.BrokerName = node.ChildNodes[11].ChildNodes[0].InnerHtml;
                    brokerSell.Buy = Convert.ToDecimal(node.ChildNodes[13].InnerHtml.Replace(",", "")) / divide;
                    brokerSell.Sell = Convert.ToDecimal(node.ChildNodes[15].InnerHtml.Replace(",", "")) / divide;
                    brokerSell.買賣超 = Convert.ToDecimal(node.ChildNodes[17].InnerHtml.Replace(",", "")) / divide;
                    brokerSell.Percent = Convert.ToDecimal(node.ChildNodes[19].InnerHtml.Replace("%", ""));

                    list.Add(brokerSell);
                }
            }
            context.BrokerTransaction.AddRange(list);
            await context.SaveChangesAsync();
            s.Stop();
            Console.WriteLine($"{stockId}, {name}, {datetime}：" + s.Elapsed.TotalSeconds);
        }

        private Prices ParseSingleHistoryPrice(string stockId, string name)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var rootNode = GetRootNoteByUrl($"{baseUrl}/Z/ZC/ZCX/ZCX_{stockId}.djhtm", false);
            var dateNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[1]/td[1]/font/div");

            if (dateNode == null)
                return null;

            var date = DateTime.Today.Year + "/" + dateNode.InnerText.Replace("最近交易日:", "").Replace("&nbsp;&nbsp;&nbsp;市值單位:百萬", "");
            var today = Convert.ToDateTime(date);
            if (today != DateTime.Today)
            {
                return null;
            }

            var openNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[2]/td[2]");
            var highNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[2]/td[4]");
            var lowNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[2]/td[6]");
            var closeNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[2]/td[8]");
            var closeValue = Convert.ToDecimal(closeNode.InnerHtml);
            var updownNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[3]/td[2]");
            var tmp = updownNode.InnerHtml
                .Replace("<p class=\"t3g1\"><p class=\"t3n1\">", "")
                .Replace("<p class=\"t3g1\"><p class=\"t3g1\">", "")
                .Replace("<p class=\"t3g1\"><p class=\"t3r1\">", "")
                .Replace("</p>\r\n", "");
            var updownValue = Convert.ToDecimal(tmp);

            var tmp2 = rootNode
                .SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[4]/td[2]")
                .InnerHtml;
            var costPercentNode = tmp2 == "N/A" ? 0 : Convert.ToDecimal(tmp2);
            var volume = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tr[2]/td[2]/table/tr/td/table[2]/tr[4]/td[8]").InnerHtml.Replace(",",""));
            var percent = Math.Round(100 * updownValue / (closeValue - updownValue),3);

            var price = new Prices
            {
                CreatedOn = DateTime.Now,
                StockId = stockId,
                Name = name,
                Datetime = today,
                Open = Convert.ToDecimal(openNode.InnerHtml),
                High = Convert.ToDecimal(highNode.InnerHtml),
                Low = Convert.ToDecimal(lowNode.InnerHtml),
                Close = closeValue,
                漲跌 = updownValue,
                漲跌百分比 = percent,
                成交量 = volume,
                成交金額 = (int)(volume * closeValue),
                本益比 = costPercentNode
            };

            s.Stop();
            //Console.WriteLine($"基本資料：{stockId}, {name}：" + s.Elapsed.TotalSeconds);

            return price;
        }
    }
}
