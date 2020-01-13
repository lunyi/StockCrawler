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
        private string optionthreeUrl = "https://histock.tw/stock/optionthree.aspx";
        private string roeUrl = "https://histock.tw/stock/financial.aspx?no={0}&t=3&st=2&q=2";

        public async Task RunAsync()
        {
            try
            {
                var context = new StockDbContext();
                var rootNode = GetRootNoteByUrl(threeUrl);

                var threeNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[1]/div[2]/table/tr");
                var futureNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[2]/div[2]/table/tr");
                var tenNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[3]/div[2]/table/tr");

                for (int i = 1; i < threeNodes.Count; i++)
                {
                    var twStock = new TwStock
                    {
                        Id = Guid.NewGuid(),

                    };

                    //threeNodes[i].
                    context.TwStock.Add(twStock);
                }

                rootNode = GetRootNoteByUrl(threeMgUrl);

                var mgNodes = rootNode.SelectNodes("/html/body/form/div[4]/div[5]/div/div[2]/div[1]/div/div/div[2]/table/tr");

                


                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        public async Task ParserRoeAsync()
        {
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
