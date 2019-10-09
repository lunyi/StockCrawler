using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DataService.Models;

namespace WebCrawler
{
    public class CnyParser : BaseParser
    {
        public Dictionary<string, string> ErrorStocks { get; set; }
        public CnyParser()
        {
            ErrorStocks = new Dictionary<string, string>();
        }

        public Prices[] ParserHistory(string stockId, string name)
        {
            try
            {
                Console.WriteLine($"{stockId} : Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                var prices = ParseHistoryPrice(stockId, name);
                ParseMargin(stockId, prices);
                ParseForeign(stockId, prices);
                ParseItrust(stockId, prices);
                ParseDealer(stockId, prices);
                return prices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{stockId} : Failed: {ex.Message}");
                Console.WriteLine($"{stockId} : Failed: {ex.StackTrace}");
                ErrorStocks.Add(stockId, name);
                return new Prices[0];
            }
        }
        private Prices[] ParseHistoryPrice(string stockId, string name)
        {
            var url = $"https://www.cnyes.com/twstock/ps_historyprice/{stockId}.htm";
            var rootNode = GetRootNoteByUrl(url);
            var ss = rootNode.SelectSingleNode(@"/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table");

            var prices = new List<Prices>();
            for (int i = 3; i < ss.ChildNodes.Count - 1; i++)
            {
                var p = new Prices();
                p.Id = Guid.NewGuid();
                p.StockId = stockId;
                p.Name = name;
                p.CreatedOn = DateTime.Now;
                p.Datetime = Convert.ToDateTime(ss.ChildNodes[i].ChildNodes[0].InnerText);
                p.Open = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[1].InnerText);
                p.High = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[2].InnerText);
                p.Low = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[3].InnerText);
                p.Close = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[4].InnerText);
                p.漲跌 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[5].InnerText);
                p.漲跌百分比 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[6].InnerText.Replace("%", ""));
                p.成交量 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[7].InnerText.Replace(",", ""));
                p.成交金額 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[8].InnerText.Replace(",", ""));
                p.本益比 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[9].InnerText);
                prices.Add(p);
            }
            return prices.ToArray();
        }
        private void ParseMargin(string stockId, Prices[] prices)
        {
            var url = $"https://www.cnyes.com/twstock/Margin/{stockId}.htm";
            var rootNode = GetRootNoteByUrl(url);
            var ss = rootNode.SelectSingleNode(@"/html/body/div[5]/div[1]/form/div[3]/div[5]/div[2]/table");

            for (int i = 3; i < ss.ChildNodes.Count - 1; i++)
            {
                var datetime = Convert.ToDateTime(ss.ChildNodes[i].ChildNodes[0].InnerText);
                var p = prices.FirstOrDefault(p => p.Datetime == datetime);
                p.融資買進 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[1].InnerText);
                p.融資賣出 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[2].InnerText);
                p.融資現償 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[3].InnerText);
                p.融資餘額 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[4].InnerText);
                var used = ss.ChildNodes[i].ChildNodes[6].InnerText.Replace("%", "");
                p.融資使用率 = used == "-" ? 0 : Convert.ToDecimal(used);
                p.融券賣出 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[7].InnerText.Replace("%", ""));
                p.融券買進 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[8].InnerText.Replace(",", ""));
                p.融券餘額 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[10].InnerText.Replace(",", ""));
                p.資券相抵 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[12].InnerText);
            }
        }

        private void ParseForeign(string stockId, Prices[] prices)
        {
            var url = $"https://www.cnyes.com/twstock/QFII/{stockId}.htm";
            var rootNode = GetRootNoteByUrl(url);
            var ss = rootNode.SelectSingleNode(@"/html/body/div[5]/div[1]/form/div[3]/div[5]/div[3]/table");

            for (int i = 5; i < ss.ChildNodes.Count - 1; i++)
            {
                var datetime = Convert.ToDateTime(ss.ChildNodes[i].ChildNodes[0].InnerText);
                var p = prices.FirstOrDefault(p => p.Datetime == datetime);
                p.外資買進 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[1].InnerText.Replace(",", ""));
                p.外資賣出 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[2].InnerText.Replace(",", ""));
                p.外資買賣超 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[3].InnerText.Replace(",", ""));
                p.外資持股 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[4].InnerText.Replace(",", ""));
                p.外資持股比例 = Convert.ToDecimal(ss.ChildNodes[i].ChildNodes[4].InnerText);
                p.尚可投資張數 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[6].InnerText.Replace(",", ""));
                p.發行張數 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[7].InnerText.Replace(",", ""));
            }
        }

        private void ParseItrust(string stockId, Prices[] prices)
        {
            var url = $"https://www.cnyes.com/twstock/itrust/{stockId}.htm";
            var rootNode = GetRootNoteByUrl(url);
            var ss = rootNode.SelectSingleNode(@"/html/body/div[5]/div[1]/form/div[3]/div[5]/div[4]/table");

            for (int i = 3; i < ss.ChildNodes.Count - 1; i++)
            {
                var datetime = Convert.ToDateTime(ss.ChildNodes[i].ChildNodes[0].InnerText);
                var p = prices.FirstOrDefault(p => p.Datetime == datetime);
                p.投信買進 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[1].InnerText.Replace(",", ""));
                p.投信賣出 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[2].InnerText.Replace(",", ""));
                p.投信買賣超 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[3].InnerText.Replace(",", ""));
            }
        }

        private void ParseDealer(string stockId, Prices[] prices)
        {
            var url = $"https://www.cnyes.com/twstock/dealer/{stockId}.htm";
            var rootNode = GetRootNoteByUrl(url);
            var ss = rootNode.SelectSingleNode(@"/html/body/div[5]/div[1]/form/div[3]/div[5]/div[4]/table");

            for (int i = 3; i < ss.ChildNodes.Count - 1; i++)
            {
                var datetime = Convert.ToDateTime(ss.ChildNodes[i].ChildNodes[0].InnerText);
                var p = prices.FirstOrDefault(p => p.Datetime == datetime);
                p.自營商買進 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[1].InnerText.Replace(",", ""));
                p.自營商賣出 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[2].InnerText.Replace(",", ""));
                p.自營商買賣超 = Convert.ToInt32(ss.ChildNodes[i].ChildNodes[3].InnerText.Replace(",", ""));
            }
        }
    }
}
