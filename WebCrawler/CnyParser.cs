using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DataService.Models;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class CnyParser : BaseParser
    {
        public ConcurrentDictionary<string, string> ErrorStocks { get; set; }

        public Prices[] ParserHistory(string stockId, string name)
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

        public Prices ParserLastDay(string stockId, string name)
        {
            try
            {
                Console.WriteLine($"{stockId} : Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                var price = ParseSingleHistoryPrice(stockId, name);
                ParseSingleNode(3, $"https://www.cnyes.com/twstock/Margin/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[2]/table", price, (htmlNode, p) => SetMargin(htmlNode, p));
                ParseSingleNode(5, $"https://www.cnyes.com/twstock/QFII/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table", price, (htmlNode, p) => SetForeign(htmlNode, p));
                ParseSingleNode(3, $"https://www.cnyes.com/twstock/itrust/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[4]/table", price, (htmlNode, p) => SetItrust(htmlNode, p));
                ParseSingleNode(3, $"https://www.cnyes.com/twstock/dealer/{stockId}.htm", "/html/body/div[5]/div[1]/form/div[3]/div[5]/div[4]/table", price, (htmlNode, p) => SetDealer(htmlNode, p));

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
            var rootNode = GetRootNoteByUrl($"https://www.cnyes.com/twstock/ps_historyprice/{stockId}.htm");
            var node = rootNode.SelectSingleNode("/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table");

            var prices = new List<Prices>();
            for (int i = 3; i < node.ChildNodes.Count - 1; i++)
            {
                prices.Add(SetPrice(node.ChildNodes[i], stockId, name));
            }
            return prices.ToArray();
        }

        public void ParseNode(int startIndex, string url, string xPzth, Prices[] prices, Action<HtmlNode, Prices> action)
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
            var rootNode = GetRootNoteByUrl($"https://www.cnyes.com/twstock/ps_historyprice/{stockId}.htm");
            var node = rootNode.SelectSingleNode("/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table");
            return SetPrice(node.ChildNodes[3], stockId, name);
        }

        public void ParseSingleNode(int startIndex, string url, string xPzth, Prices price, Action<HtmlNode, Prices> action)
        {
            var rootNode = GetRootNoteByUrl(url);
            var node = rootNode.SelectSingleNode(xPzth);

            if (node.ChildNodes.Count > startIndex)
            {
                var datetime = Convert.ToDateTime(node.ChildNodes[startIndex].ChildNodes[0].InnerText);
                if (price.Datetime == datetime)
                {
                    action(node.ChildNodes[startIndex], price);
                }
            }
        }
    }
}
