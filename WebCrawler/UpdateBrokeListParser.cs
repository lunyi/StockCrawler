using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler
{
    public class UpdateBrokeListParser2 : BaseParser
    {
        public async Task RunAsync()
        {
            var context = new StockDbContext();

            var url = "https://fubon-ebrokerdj.fbs.com.tw/z/zg/zgb/zgb0.djhtm";
            var rootNode = GetRootNoteByUrl(url, false);
            var node1 = rootNode.SelectSingleNode("//*[@id=\"oScrollHead\"]");
            var node2 = rootNode.SelectSingleNode("//*[@id=\"oScrollHead\"]/td");
            var node3 = rootNode.SelectSingleNode("//*[@id=\"oScrollHead\"]/td/select[1]");
            for (int i = 0; i < node1.ChildNodes.Count; i++)
            {
                var node = node1.ChildNodes[i];
            }           
        }

        public async Task Run2Async()
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new StockDbContext();

            var prices = context.Prices
                .Where(p => p.StockId == "1409")
                .OrderByDescending(p => p.Datetime)
                .ToList();

            for (int i = 0; i < prices.Count; i++)
            {
                var date = prices[i].Datetime.ToString("yyyy-M-d");
                var url = $@"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco.djhtm?a={prices[i].StockId}&e={date}&f={date}";
                var rootNode = GetRootNoteByUrl(url, false);
                var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

                for (int j = 6; j < nodes.Count - 2; j++)
                {
                    var node = nodes[j];
                    try
                    {
                        var broker = GetBrokerInfo(node, 1);
                        UpdateBroke(broker, context, date);
                        var broker2 = GetBrokerInfo(node, 11);
                        UpdateBroke(broker2, context, date);

                        if (node.ChildNodes[1].InnerHtml.Contains("合計"))
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        private void UpdateBroke(BrokerInfo broker, StockDbContext context, string date)
        {
            if (broker != null)
            {
                var oldBroker = context.Broker.FirstOrDefault(p => p.BHID == broker.BHID);
                if (oldBroker != null)
                {
                    oldBroker.BHID = broker.BHID;
                    oldBroker.b = broker.b;
                    context.SaveChanges();
                    Console.WriteLine(broker.BHID + " " + broker.Name + $"not found  {date}  {broker.Val}");
                }
            }
        }
        private BrokerInfo GetBrokerInfo(HtmlNode node, int index)
        {
            try
            {
                var html = node.ChildNodes[index].InnerHtml.Replace("<a href=\"/z/zc/zco/zco0/zco0.djhtm?", "");
                var name = node.ChildNodes[index].ChildNodes[0].InnerHtml.Replace("證券","");
                var k = html.IndexOf('>');
                var tmp = html.Substring(0, k - 1);
                var tmp2 = tmp.Split('&');
                var a = tmp2[0].Split("=")[1];
                var b = tmp2[1].Split("=")[1];
                var bhid = tmp2[2].Split("=")[1];
                return new BrokerInfo(b, bhid, name, node.ChildNodes[index].InnerHtml);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}