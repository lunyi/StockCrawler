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
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class CnyParser : BaseParser
    {
        public ConcurrentDictionary<string, string> ErrorStocks { get; set; }

        [Obsolete]
        public async Task RunAsync()
        {
            var context = new StockDbContext();
            var s = Stopwatch.StartNew();
            s.Start();

            var parser = new CnyParser();
            var stocks = context.Stocks.FromSqlRaw(GetSql()).ToList();

            foreach (var item in stocks)
            {
                await ExecuteLastAsync(parser, context, item.StockId, item.Name);
            }

            var dd = await context.Prices.Select(p => p.Datetime).Distinct().OrderByDescending(p => p).Take(2).ToArrayAsync();

            context.Database.ExecuteSqlCommand(GetSqlToUpdate發行張數(dd[0].ToString("yyyy-MM-dd"), dd[1].ToString("yyyy-MM-dd")));
            context.Database.ExecuteSqlCommand(GetSqlToUpdate());

            s.Stop();
            Console.WriteLine($"Spend times {s.Elapsed.TotalMinutes} minutes.");
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

        private static string GetSql()
        {
            return @$"
  select s.* from [Stocks]  s 
  left join (select * from [Prices] where [Datetime] = '{DateTime.Today.ToString("yyyy/MM/dd")}') p on s.StockId = p.StockId
  where  s.Status = 1 and p.Id is null
  order by s.StockId desc";
        }

        private async Task ExecuteHistoryAsync(CnyParser parser, StockDbContext context, string stockId, string name)
        {
            var prices = parser.ParserHistory(stockId, name);

            foreach (var price in prices)
            {
                var p = context.Prices.FirstOrDefault(p => p.Datetime == price.Datetime && p.StockId == stockId);
                if (p == null)
                {
                    context.Prices.Add(price);
                }
            }
            Console.WriteLine($"Finished: {stockId} {name}");

            await context.SaveChangesAsync();
        }

        private async Task ExecuteLastAsync(CnyParser parser, StockDbContext context, string stockId, string name)
        {
            var price = parser.ParserLastDay(stockId, name);

            if (price == null)
            {
                return;
            }
            var p = context.Prices.FirstOrDefault(p => p.Datetime == price.Datetime && p.StockId == stockId);
            if (p == null)
            {
                context.Prices.Add(price);
                var stock = await context.Stocks.FirstOrDefaultAsync(p => p.StockId == stockId);
                stock.Description = price.Close.ToString();
                context.Entry<Stocks>(stock).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }

        [Obsolete]
        private RawSqlString GetSqlToUpdate()
        {
            return new RawSqlString(@"
Update  [Prices] set [投信買賣超] = (投信買進 - 投信賣出), [外資買賣超] = (外資買進 - 外資賣出), [自營商買賣超] = (自營商買進 - 自營商賣出)
where ([投信買賣超] != (投信買進 - 投信賣出)) or  ([外資買賣超] != (外資買進 - 外資賣出)) or  ([自營商買賣超] != (自營商買進 - 自營商賣出))
");
        }

        [Obsolete]
        private RawSqlString GetSqlToUpdate發行張數(string firstDatetime, string secondDatetime)
        {
            return new RawSqlString(@$"
 update [Prices] set [發行張數] = c.發行張數2
 from (
select 
  a.stockId, a.Name, 
  a.[Datetime] as [Datetime1], 
  a.發行張數 as 發行張數1, 
  b.Datetime as [Datetime2], 
  b.發行張數 as 發行張數2 
from (
SELECT *
  FROM [StockDb].[dbo].[Prices]
  where [Datetime] = '{firstDatetime}' and ([發行張數] is null or [發行張數] = 0))  a 
  join (SELECT *
  FROM [StockDb].[dbo].[Prices]
  where [Datetime] = '{secondDatetime}')  b on a.StockId = b.StockId) c 

   where [Datetime] = '{firstDatetime}' and  ([發行張數] is null or [發行張數] = 0)

");
        }
        private Prices[] ParserHistory(string stockId, string name)
        {
            try
            {
                Console.WriteLine($"{stockId} : Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                var prices = ParseHistoryPrice(stockId, name);
                ParseNode(3, $"https://www.cnyes.com/twstock/Margin/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[2]/table", prices, (htmlNode, p) => SetMargin(htmlNode, p));
                ParseNode(5, $"https://www.cnyes.com/twstock/QFII/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table", prices, (htmlNode, p) => SetForeign(htmlNode, p));
                ParseNode(3, $"https://www.cnyes.com/twstock/itrust/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[4]/table", prices, (htmlNode, p) => SetItrust(htmlNode, p));
                ParseNode(3, $"https://www.cnyes.com/twstock/dealer/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[4]/table", prices, (htmlNode, p) => SetDealer(htmlNode, p));

                return prices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{stockId} : Failed: {ex.Message}");
                Console.WriteLine($"{stockId} : Failed: {ex.StackTrace}");
                ErrorStocks.AddOrUpdate(stockId, name, (key, oldValue) => name);
                return new Prices[0];
            }
        }

        private Prices ParserLastDay(string stockId, string name)
        {
            try
            {
                Console.WriteLine($"{stockId} : Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                var price = ParseSingleHistoryPrice(stockId, name);
                ParseSingleNode(3, $"https://www.cnyes.com/twstock/Margin/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[2]/table", price, (htmlNode, p) => SetMargin(htmlNode, p));
                ParseSingleNode(5, $"https://www.cnyes.com/twstock/QFII/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[3]/table", price, (htmlNode, p) => SetForeign(htmlNode, p));
                ParseSingleNode(3, $"https://www.cnyes.com/twstock/itrust/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[4]/table", price, (htmlNode, p) => SetItrust(htmlNode, p));
                ParseSingleNode(3, $"https://www.cnyes.com/twstock/dealer/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[4]/table", price, (htmlNode, p) => SetDealer(htmlNode, p));
                ParseTech(stockId, price);
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

        [Obsolete]
        public async Task ParserMarginAsync()
        {
            var context = new StockDbContext();
            var prices = context.Prices.Where(p => p.Datetime == DateTime.Today && p.融券買進 == null)
                .OrderByDescending(p => p.StockId)
                .ToList();

            for (int i = 0; i < prices.Count; i++)
            {
                var stockId = prices[i].StockId;
                var name = prices[i].Name;

                try
                {
                    ParseSingleNode(3, $"https://www.cnyes.com/twstock/Margin/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[2]/table", prices[i], (htmlNode, p) => SetMargin(htmlNode, p));
                    ParseSingleNode(5, $"https://www.cnyes.com/twstock/QFII/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[3]/table", prices[i], (htmlNode, p) => SetForeign(htmlNode, p));
                    ParseSingleNode(3, $"https://www.cnyes.com/twstock/itrust/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[4]/table", prices[i], (htmlNode, p) => SetItrust(htmlNode, p));
                    ParseSingleNode(3, $"https://www.cnyes.com/twstock/dealer/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[2]/div[5]/div[4]/table", prices[i], (htmlNode, p) => SetDealer(htmlNode, p));
                    await context.SaveChangesAsync();
                    Console.WriteLine(stockId + "::" + name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(stockId + "::" + name + "::" + ex);
                }
            }

            var dd = await context.Prices.Select(p => p.Datetime).Distinct().OrderByDescending(p => p).Take(2).ToArrayAsync();

            context.Database.ExecuteSqlCommand(GetSqlToUpdate發行張數(dd[0].ToString("yyyy-MM-dd"), dd[1].ToString("yyyy-MM-dd")));
            context.Database.ExecuteSqlCommand(GetSqlToUpdate());
        }

        public CnyParser()
        {
            ErrorStocks = new ConcurrentDictionary<string, string>();
        }

        private Prices SetPrice(HtmlNode htmlNode, string stockId, string name)
        {
            return new Prices
            {
                Id = Guid.NewGuid(),
                StockId = stockId,
                Name = name,
                CreatedOn = DateTime.Now,
                Datetime = Convert.ToDateTime(htmlNode.ChildNodes[0].InnerText),
                Open = Convert.ToDecimal(htmlNode.ChildNodes[1].InnerText),
                High = Convert.ToDecimal(htmlNode.ChildNodes[2].InnerText),
                Low = Convert.ToDecimal(htmlNode.ChildNodes[3].InnerText),
                Close = Convert.ToDecimal(htmlNode.ChildNodes[4].InnerText),
                漲跌 = Convert.ToDecimal(htmlNode.ChildNodes[5].InnerText),
                漲跌百分比 = Convert.ToDecimal(htmlNode.ChildNodes[6].InnerText.Replace("%", "")),
                成交量 = Convert.ToInt32(htmlNode.ChildNodes[7].InnerText.Replace(",", "")),
                成交金額 = Convert.ToInt32(htmlNode.ChildNodes[8].InnerText.Replace(",", "")),
                本益比 = Convert.ToDecimal(htmlNode.ChildNodes[9].InnerText)
            }; 
        }

        private void SetMargin(HtmlNode htmlNode, Prices price)
        {
            price.融資買進 = Convert.ToInt32(htmlNode.ChildNodes[1].InnerText);
            price.融資賣出 = Convert.ToInt32(htmlNode.ChildNodes[2].InnerText);
            price.融資現償 = Convert.ToInt32(htmlNode.ChildNodes[3].InnerText);
            price.融資餘額 = Convert.ToInt32(htmlNode.ChildNodes[4].InnerText);
            var used = htmlNode.ChildNodes[6].InnerText.Replace("%", "");
            price.融資使用率 = used == "-" ? 0 : Convert.ToDecimal(used);
            price.融券賣出 = Convert.ToInt32(htmlNode.ChildNodes[7].InnerText.Replace("%", ""));
            price.融券買進 = Convert.ToInt32(htmlNode.ChildNodes[8].InnerText.Replace(",", ""));
            price.融券餘額 = Convert.ToInt32(htmlNode.ChildNodes[10].InnerText.Replace(",", ""));
            price.資券相抵 = Convert.ToInt32(htmlNode.ChildNodes[12].InnerText);
        }

        private void SetForeign(HtmlNode htmlNode, Prices price)
        {
            price.外資買進 = Convert.ToInt32(htmlNode.ChildNodes[1].InnerText.Replace(",", ""));
            price.外資賣出 = Convert.ToInt32(htmlNode.ChildNodes[2].InnerText.Replace(",", ""));
            price.外資買賣超 = Convert.ToInt32(htmlNode.ChildNodes[3].InnerText.Replace(",", ""));
            price.外資持股 = Convert.ToInt32(htmlNode.ChildNodes[4].InnerText.Replace(",", ""));
            price.外資持股比例 = Convert.ToDecimal(htmlNode.ChildNodes[5].InnerText);
            price.尚可投資張數 = Convert.ToInt32(htmlNode.ChildNodes[6].InnerText.Replace(",", ""));
            price.發行張數 = Convert.ToInt32(htmlNode.ChildNodes[7].InnerText.Replace(",", ""));
        }

        private void SetItrust(HtmlNode htmlNode, Prices price)
        {
            price.投信買進 = Convert.ToInt32(htmlNode.ChildNodes[1].InnerText.Replace(",", ""));
            price.投信賣出 = Convert.ToInt32(htmlNode.ChildNodes[2].InnerText.Replace(",", ""));
            price.投信買賣超 = Convert.ToInt32(htmlNode.ChildNodes[3].InnerText.Replace(",", ""));
        }

        private void SetDealer(HtmlNode htmlNode, Prices price)
        {
            price.自營商買進 = Convert.ToInt32(htmlNode.ChildNodes[1].InnerText.Replace(",", ""));
            price.自營商賣出 = Convert.ToInt32(htmlNode.ChildNodes[2].InnerText.Replace(",", ""));
            price.自營商買賣超 = Convert.ToInt32(htmlNode.ChildNodes[3].InnerText.Replace(",", ""));
        }

        private Prices[] ParseHistoryPrice(string stockId, string name)
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var rootNode = GetRootNoteByUrl($"https://www.cnyes.com/twstock/ps_historyprice/{stockId}.htm");
            var node = rootNode.SelectSingleNode("/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table");

            var prices = new List<Prices>();
            for (int i = 3; i < node.ChildNodes.Count - 1; i++)
            {
                prices.Add(SetPrice(node.ChildNodes[i], stockId, name));
            }

            s.Stop();
            Console.WriteLine("基本股價：" + s.Elapsed.TotalSeconds);
            return prices.ToArray();
        }
        public void ParseTrust(string stockId, Prices price) 
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var datetime = DateTime.Now.ToString("yyyy-MM-dd");
            var url = $@"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zcj/zcj_{stockId}.djhtm";

            var rootNode = GetRootNoteByUrl(url, false);

            price.董監持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[3]/td[2]").InnerHtml.Replace(",", ""));
            price.外資持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[4]/td[2]").InnerHtml.Replace(",", ""));
            price.投信持股 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/table/tr/td/table/tr/td/table/tr[5]/td[2]").InnerHtml.Replace(",", ""));
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
                var rootNode = GetRootNoteByUrl($"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco_{stockId}_{index}.djhtm", false);
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

            var url = $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco.djhtm?a={stockId}&e={datetime}&f={datetime}";
            var rootNode = GetRootNoteByUrl(url, false);
            var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

            if (nodes.Count < 8)
            {
                return;
            }
            var divide = 1;
            if (nodes[nodes.Count - 3].ChildNodes[1].InnerHtml == "合計買超股數")
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

        private void ParseTech(string stockId, Prices price)
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var rootNode = GetRootNoteByUrl($"https://www.cnyes.com/twstock/Technical/{stockId}.htm");
            var node = rootNode.SelectSingleNode("/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table[1]");

            if (node != null)
            {
                price.MA3 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[2].InnerText);
                price.MA5 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[3].InnerText);
                price.MA10 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[4].InnerText);
                price.MA20 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[5].InnerText);
                price.MA60 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[6].InnerText);
                price.MA120 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[7].InnerText);
                price.MA240 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[8].InnerText);

                node = rootNode.SelectSingleNode("/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table[2]");

                price.VMA3 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[2].InnerText);
                price.VMA5 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[3].InnerText);
                price.VMA10 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[4].InnerText);
                price.VMA20 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[5].InnerText);
                price.VMA60 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[6].InnerText);
                price.VMA120 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[7].InnerText);
                price.VMA240 = Convert.ToDecimal(node.ChildNodes[1].ChildNodes[8].InnerText);
            }

            s.Stop();
            Console.WriteLine("MA, VMA：" + s.Elapsed.TotalSeconds);
        }

        private void ParseNode(int startIndex, string url, string xPzth, Prices[] prices, Action<HtmlNode, Prices> action)
        {
            var rootNode = GetRootNoteByUrl(url);
            var node = rootNode.SelectSingleNode(xPzth);

            for (int i = startIndex; i < node.ChildNodes.Count - 1; i++)
            {
                var datetime = Convert.ToDateTime(node.ChildNodes[i].ChildNodes[0].InnerText);
                var p = prices.FirstOrDefault(p => p.Datetime == datetime);
                if (p != null)
                {
                    action(node.ChildNodes[i], p);
                }
            }
        }

        private Prices ParseSingleHistoryPrice(string stockId, string name)
        {
            var rootNode = GetRootNoteByUrl($"https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{stockId}.djhtm");
            var htmlNode = rootNode.SelectSingleNode("//*[@id=\"_historyDataTable\"]/div[2]/div[2]/table/tbody/tr[1]/td[1]");
            var dateNode = rootNode.SelectSingleNode("//*[@id=\"SysJustIFRAMEDIV\"]/table/tbody/tr[2]/td[2]/table/tbody/tr/td/table[1]/tbody/tr/td/table[2]/tbody/tr[1]/td/font/div");
            var date = dateNode.InnerText.Replace("最近交易日:", "").Replace("市值單位:百萬", "");
            return new Prices
            {
                Id = Guid.NewGuid(),
                StockId = stockId,
                Name = name,
                CreatedOn = DateTime.Now,
                Datetime = Convert.ToDateTime(htmlNode.ChildNodes[0].InnerText),
                Open = Convert.ToDecimal(htmlNode.ChildNodes[1].InnerText),
                High = Convert.ToDecimal(htmlNode.ChildNodes[2].InnerText),
                Low = Convert.ToDecimal(htmlNode.ChildNodes[3].InnerText),
                Close = Convert.ToDecimal(htmlNode.ChildNodes[4].InnerText),
                漲跌 = Convert.ToDecimal(htmlNode.ChildNodes[5].InnerText),
                漲跌百分比 = Convert.ToDecimal(htmlNode.ChildNodes[6].InnerText.Replace("%", "")),
                成交量 = Convert.ToInt32(htmlNode.ChildNodes[7].InnerText.Replace(",", "")),
                //成交金額 = Convert.ToInt32(htmlNode.ChildNodes[8].InnerText.Replace(",", "")),
                //本益比 = Convert.ToDecimal(htmlNode.ChildNodes[9].InnerText)
            };
        }

        private void ParseSingleNode(int startIndex, string url, string xPath, Prices price, Action<HtmlNode, Prices> action)
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var rootNode = GetRootNoteByUrl(url);
            var node = rootNode.SelectSingleNode(xPath);

            if (node != null && node.ChildNodes.Count > startIndex)
            {
                var datetime = Convert.ToDateTime(node.ChildNodes[startIndex].ChildNodes[0].InnerText);
                if (price.Datetime == datetime)
                {
                    action(node.ChildNodes[startIndex], price);
                }
            }
            s.Stop();
            Console.WriteLine(url + "：" +s.Elapsed.TotalSeconds);
        }
    }
}
