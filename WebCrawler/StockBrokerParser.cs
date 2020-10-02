using DataService.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class StockBrokerParser : BaseParser
    {
        public async Task RunAsync()
        {
            var s = Stopwatch.StartNew();
            s.Start();
            var context = new StockDbContext();

            var stocks = context.Stocks.FromSqlRaw(GetStockIdbyString()).ToArray();
            //var stocks = context.Stocks.Where(p => p.Status == 1).ToArray();
            var startDate = "2020-1-1";
            var endDate = "2020-9-30";
            //var testStockIds = new[] { "2903", "2012", "1110" };

            //var testStockIds = new string[] { ""};

            for (int i = 0; i < stocks.Length; i++)
            {
                var stockId = stocks[i].StockId;
                var name = stocks[i].Name;

                //if (!testStockIds.Contains(stockId))
                //{
                //    continue;
                //}

                //var tmpss = context.StockBrokers.Where(p => p.StockId == stockId).ToArray();

                //for (int kk = 0; kk < tmpss.Length; kk++)
                //{
                //    context.Entry(tmpss[kk]).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                //}
                //await context.SaveChangesAsync();

                //var stockId = "2012";
                //var name = "春雨";
                //var brokers = new List<BrokerInfo>();
                var url = $@"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco.djhtm?a={stockId}&e={startDate}&f={endDate}";
                var rootNode = GetRootNoteByUrl(url, false);
                var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

                for (int j = 6; j < nodes.Count - 2; j++)
                {
                    var node = nodes[j];
                    try
                    {
                        if (node.ChildNodes[1].InnerHtml.Contains("合計"))
                        {
                            break;
                        }

                        var broker = GetBrokerInfo(node);
                        var url2 = $@"http://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={stockId}&BHID={broker.BHID}&b={broker.b}&C=1&D={startDate}&E={endDate}&ver=V3";
                        var rootNode2 = GetRootNoteByUrl(url2, false);
                        var nodes2 = rootNode2.SelectNodes("/html/body/div/table/tr[2]/td[2]/form/table/tr/td/table/tr[6]/td/table/tr");
                        int postiveCount = 0;
                        int negativeCount = 0;
                        var postiveVol = 0;
                        var negativeVol = 0;
                        HtmlNode postiveNode = null;
                        HtmlNode negativeNode = null;
                        DateTime? startDate1 = DateTime.Now;
                        for (int k = 1; k < nodes2.Count; k++)
                        {
                            var tmpNode = nodes2[k];
                            var tmpVol = Convert.ToInt32(tmpNode.ChildNodes[9].InnerHtml.Replace(",",""));
                            var tmpDatetime = Convert.ToDateTime(tmpNode.ChildNodes[1].InnerHtml); 

                            if (tmpVol > 0)
                            {
                                if (postiveNode == null)
                                {
                                    postiveNode = tmpNode;
                                    postiveCount = 1;
                                    postiveVol = tmpVol;
                                    startDate1 = tmpDatetime;
                                }
                                else
                                {
                                    postiveCount++;
                                    postiveVol += tmpVol;
                                }

                                if (negativeNode != null)
                                {
                                    negativeNode = null;
                                    if (negativeCount > 15)
                                    {
                                        //TODO : Insert DB
                                        var sb = new StockBrokers
                                        {
                                            Id = Guid.NewGuid(),
                                            StockId = stockId,
                                            Name = name,
                                            BrokerId = broker.b,
                                            BrokerName = broker.Name,
                                            StartDate  = tmpDatetime,
                                            EndDate = startDate1.Value,
                                            Volume = negativeVol,
                                            Count = negativeCount
                                        };

                                        context.StockBrokers.Add(sb);
                                        await context.SaveChangesAsync();
                                        Console.WriteLine($"{stockId} {name}");

                                        negativeCount = 0;
                                        negativeVol = 0;
                                        startDate1 = null;
                                    }
                                }
                            }
                            else if (tmpVol < 0)
                            {
                                if (postiveNode != null)
                                {
                                    postiveNode = null;
                                    if (postiveCount > 15)
                                    {
                                        //TODO : Insert DB
                                        var sb = new StockBrokers
                                        {
                                            Id = Guid.NewGuid(),
                                            StockId = stockId,
                                            Name = name,
                                            BrokerId = broker.b,
                                            BrokerName = broker.Name,
                                            StartDate = tmpDatetime,
                                            EndDate = startDate1.Value,
                                            Volume = postiveVol,
                                            Count = postiveCount
                                        };

                                        context.StockBrokers.Add(sb);
                                        await context.SaveChangesAsync();
                                        Console.WriteLine($"{stockId} {name}");

                                        postiveCount = 0;
                                        postiveVol = 0;
                                        startDate1 = null;
                                    }
                                }

                                if (negativeNode == null)
                                {
                                    negativeNode = tmpNode;
                                    negativeCount = 1;
                                    negativeVol = tmpVol;
                                    startDate1 = tmpDatetime;
                                }
                                else 
                                {
                                    negativeCount++;
                                    negativeVol += tmpVol;
                                }
                            }

                            if (k == nodes2.Count - 1 && (postiveCount > 15 || negativeCount > 15 ))
                            {
                                var sb = new StockBrokers
                                {
                                    Id = Guid.NewGuid(),
                                    StockId = stockId,
                                    Name = name,
                                    BrokerId = broker.b,
                                    BrokerName = broker.Name,
                                    StartDate = tmpDatetime,
                                    EndDate = startDate1.Value,
                                    Volume = postiveVol > 0 ? postiveVol : negativeVol,
                                    Count = postiveCount > 0 ? postiveCount : negativeCount
                                };

                                context.StockBrokers.Add(sb);
                                await context.SaveChangesAsync();
                                Console.WriteLine($"{stockId} {name}");

                                postiveCount = 0;
                                postiveVol = 0;
                                startDate1 = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private string GetStockIdbyString()
        {
            return $@"
select * from Stocks s 
where s.Status = 1 and s.StockId > (select max(StockId) from [StockBrokers])
";
        }
        private BrokerInfo GetBrokerInfo(HtmlNode node)
        {
            try
            {
                var html = node.ChildNodes[1].InnerHtml.Replace("<a href=\"/z/zc/zco/zco0/zco0.djhtm?", "");
                var k = html.IndexOf('>');
                var tmp = html.Substring(0, k - 1);
                var tmp2 = tmp.Split('&');
                var a = tmp2[0].Split("=")[1];
                var b = tmp2[1].Split("=")[1];
                var bhid = tmp2[2].Split("=")[1];
                var name = html.Substring(k + 1, html.IndexOf('<') - k - 1);
                return new BrokerInfo(b, bhid, name);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class BrokerInfo
    {
        public BrokerInfo(string _b, string bhid, string name)
        {
            b = _b;
            BHID = bhid;
            Name = name;
        }
        public string b { get; set; }
        public string BHID { get; set; }
        public string Name { get; set; }
        public string Val { get; set; }
    }
}
