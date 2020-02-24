using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class HiStockParser : BaseParser
    {
        private string threeUrl = "https://histock.tw/stock/three.aspx";
        private string threeMgUrl = "https://histock.tw/stock/three.aspx?m=mg";
        private string indicatorUrl = "https://histock.tw/stock/indicator.aspx";
        private string optionUrl = "https://histock.tw/stock/optionthree.aspx";


        public async Task RunAsync()
        {
            var context = new StockDbContext();
            var rootNode = GetRootNoteByUrl(threeUrl);

            var threeNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[1]/div[2]/table/tr");
            var futureNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[2]/div[2]/table/tr");
            var tenNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[3]/div[2]/table/tr");

            for (int i = 1; i < threeNodes.Count; i++)
            {
                try
                {
                    var threeNode = threeNodes[i];
                    var futureNode = futureNodes[i];
                    var tenNode = tenNodes[i];
                    int year = threeNode.ChildNodes[0].InnerHtml.Contains("01/") ? DateTime.Now.Year : DateTime.Now.Year - 1;

                    var datetime = $"{year}/{threeNode.ChildNodes[0].ChildNodes[0].InnerHtml}";

                    var dd = context.TwStock.FirstOrDefault(p => p.Datetime == Convert.ToDateTime(datetime));
                    if (dd == null)
                    //if (datetime == DateTime.Now.ToString("yyyy/MM/dd"))
                    {
                        var twStock = new TwStock();
                        twStock.Id = Guid.NewGuid();
                        twStock.Datetime = Convert.ToDateTime(datetime);

                        ParserITrust(threeNode, futureNode, tenNode, twStock);
                        ParserMargin(datetime, twStock);
                        ParserOption(datetime, twStock);
                        ParserUpDownCount(twStock);

                        context.TwStock.Add(twStock);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        var twStock = dd;
                        ParserITrust(threeNode, futureNode, tenNode, twStock);
                        ParserMargin(datetime, twStock);
                        ParserOption(datetime, twStock);
                        ParserUpDownCount(twStock);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
            }
        }

        public async Task RunSingleAsync()
        {
            var context = new StockDbContext();
            var rootNode = GetRootNoteByUrl(threeUrl);

            var threeNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[1]/div[2]/table/tr");
            var futureNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[2]/div[2]/table/tr");
            var tenNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[3]/div[2]/table/tr");

            try
            {
                var datetime = $"{DateTime.Now.Year}/{threeNodes[1].ChildNodes[0].ChildNodes[0].InnerHtml}";
                Console.WriteLine($"{datetime} OK");

                if (datetime == DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    var twStock = new TwStock();
                    twStock.Id = Guid.NewGuid();
                    twStock.Datetime = Convert.ToDateTime(datetime);

                    ParserITrust(threeNodes[1], futureNodes[1], tenNodes[1], twStock);
                    ParserMargin(datetime, twStock);
                    ParserOption(datetime, twStock);
                    ParserUpDownCount(twStock);

                    context.TwStock.Add(twStock);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        private void ParserITrust(HtmlNode threeNode, HtmlNode futureNode, HtmlNode tenNode, TwStock twStock)
        {
            twStock.外資買賣超 = Convert.ToDecimal(threeNode.ChildNodes[1].ChildNodes[0].InnerHtml);
            twStock.投信買賣超 = Convert.ToDecimal(threeNode.ChildNodes[2].ChildNodes[0].InnerHtml);
            twStock.自營總 = Convert.ToDecimal(threeNode.ChildNodes[3].ChildNodes[0].InnerHtml);
            twStock.自營自買 = Convert.ToDecimal(threeNode.ChildNodes[4].ChildNodes[0].InnerHtml);
            twStock.自營避險 = Convert.ToDecimal(threeNode.ChildNodes[5].ChildNodes[0].InnerHtml);
            twStock.總計 = Convert.ToDecimal(threeNode.ChildNodes[6].ChildNodes[0].InnerHtml);

            twStock.外資未平倉 = Convert.ToDecimal(futureNode.ChildNodes[1].ChildNodes[0].InnerHtml.Replace(",", ""));
            twStock.投信未平倉 = Convert.ToDecimal(futureNode.ChildNodes[2].ChildNodes[0].InnerHtml.Replace(",", ""));
            twStock.自營未平倉 = Convert.ToDecimal(futureNode.ChildNodes[3].ChildNodes[0].InnerHtml.Replace(",", ""));
            twStock.總計未平倉 = Convert.ToDecimal(futureNode.ChildNodes[4].ChildNodes[0].InnerHtml.Replace(",", ""));

            twStock.前五大 = Convert.ToDecimal(tenNode.ChildNodes[1].ChildNodes[0].InnerHtml.Replace(",", ""));
            twStock.前十大 = Convert.ToDecimal(tenNode.ChildNodes[2].ChildNodes[0].InnerHtml.Replace(",", ""));
            twStock.前五特 = Convert.ToDecimal(tenNode.ChildNodes[3].ChildNodes[0].InnerHtml.Replace(",", ""));
            twStock.前十特 = Convert.ToDecimal(tenNode.ChildNodes[4].ChildNodes[0].InnerHtml.Replace(",", ""));
        }

        private void ParserMargin(string datetime, TwStock twStock)
        {
            var rootNode = GetRootNoteByUrl(threeMgUrl);
            var mgNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[1]/div/div/div[2]/table/tr");

            for (int i = 1; i < mgNodes.Count; i++)
            {
                var mgNode = mgNodes[i];

                int year = mgNode.ChildNodes[0].InnerHtml.Contains("01/") ? DateTime.Now.Year : DateTime.Now.Year - 1;
                var _datetime = $"{year}/{mgNode.ChildNodes[0].ChildNodes[0].InnerHtml}";

                if (_datetime == datetime)
                {
                    twStock.融資餘額 = Convert.ToDecimal(mgNode.ChildNodes[1].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.融資增加 = Convert.ToDecimal(mgNode.ChildNodes[2].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.融券餘額 = Convert.ToDecimal(mgNode.ChildNodes[3].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.融券增加 = Convert.ToDecimal(mgNode.ChildNodes[4].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.收盤價 = Convert.ToDecimal(mgNode.ChildNodes[5].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.成交量 = Convert.ToDecimal(mgNode.ChildNodes[7].ChildNodes[0].InnerHtml.Replace(",", ""));
                    break;
                }
            }
        }

        private void ParserOption(string datetime, TwStock twStock)
        {
            var rootNode = GetRootNoteByUrl(optionUrl);
            var optionNodes = rootNode.SelectNodes("//*[@id='CPHB1_pnlTabl1']/div/div/div[2]/table/tr");

            for (int i = 2; i < optionNodes.Count; i++)
            {
                var optionNode = optionNodes[i];
                int year = optionNode.ChildNodes[0].InnerHtml.Contains("01/") ? DateTime.Now.Year : DateTime.Now.Year - 1;
                var _datetime = $"{year}/{optionNode.ChildNodes[0].ChildNodes[0].InnerHtml}";

                if (_datetime == datetime)
                {
                    twStock.漲跌 = Convert.ToDecimal(optionNode.ChildNodes[2].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.漲跌百分比 = Convert.ToDecimal(optionNode.ChildNodes[3].ChildNodes[0].InnerHtml.Replace("▲", "").Replace("▼", ""));
                    twStock.外資選擇權交易口數 = Convert.ToInt32(optionNode.ChildNodes[4].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.自營選擇權交易口數 = Convert.ToInt32(optionNode.ChildNodes[6].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.外資選擇權未平倉口數 = Convert.ToInt32(optionNode.ChildNodes[8].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.自營選擇權未平倉口數 = Convert.ToInt32(optionNode.ChildNodes[12].ChildNodes[0].InnerHtml.Replace(",", ""));
                    twStock.交易口數PC比 = Convert.ToDecimal(optionNode.ChildNodes[7].ChildNodes[0].InnerHtml.Replace("%", ""));
                    twStock.未平倉口數PC比 = Convert.ToDecimal(optionNode.ChildNodes[14].ChildNodes[0].InnerHtml.Replace("%", ""));
                    break;
                }
            }
        }

        private void ParserUpDownCount(TwStock twStock)
        {
            var rootNode = GetRootNoteByUrl(indicatorUrl);
            twStock.漲停家數 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='CPHB1_ctl00_fv_raiseLimit']").ChildNodes[0].InnerHtml);
            twStock.跌停家數 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='CPHB1_ctl00_fv_fallLimit']").ChildNodes[0].InnerHtml);
            twStock.上漲家數 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='CPHB1_ctl00_fv_raiseK']").ChildNodes[0].InnerHtml);
            twStock.下跌家數 = Convert.ToInt32(rootNode.SelectSingleNode("//*[@id='CPHB1_ctl00_fv_fallK']").ChildNodes[0].InnerHtml);
        }



        public async Task ParserRoeAsync()
        {
            var roeUrl = "https://histock.tw/stock/financial.aspx?no={0}&t=3&st=2&q=2";
            var context = new StockDbContext();
            var stocks = context.Stocks.Where(p => p.Status == 1).OrderBy(p=>p.StockId).ToArray();

            for (int i = 0; i < stocks.Length; i++)
            {
                try
                {
                    var rootNode = GetRootNoteByUrl($@"https://histock.tw/stock/financial.aspx?no={stocks[i].StockId}&t=3&st=2&q=2");

                    var roeNode = rootNode.SelectNodes("/html/body/form/div[4]/div[4]/div/div[1]/div[2]/div/div[4]/div[2]/div/div/table/tr[2]");

                    var season = roeNode[0].ChildNodes[1].InnerHtml;
                    stocks[i].ROE = Convert.ToDecimal(roeNode[0].ChildNodes[2].InnerHtml.Replace("%",""));
                    stocks[i].ROA = Convert.ToDecimal(roeNode[0].ChildNodes[3].InnerHtml.Replace("%", ""));

                    await context.SaveChangesAsync();
                    Console.WriteLine($"{stocks[i].StockId} OK!");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
            }
        }
    }
}
