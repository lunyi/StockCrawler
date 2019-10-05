using ConsoleApp.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Update Model
            //Scaffold-DbContext "Server=localhost;Database=StockDb;User ID=sa;Password=sa;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -UseDatabaseNames

            //var context = new StockDbContext();

            //var stock = new Stocks()
            //{
            //};



            string url = "http://5850web.moneydj.com/z/zc/zcx/zcxNew_2498.djhtm";
            string stockDepositUrl = "https://statementdog.com/analysis/tpe/6153";

            string stockHealthCheckUrl = "https://statementdog.com/analysis/tpe/6153/stock-health-check";

            var rootNode = GetRootNoteByUrl(stockHealthCheckUrl);

            var ss = rootNode.SelectSingleNode(@"/html/body");

            var ss1 = rootNode.SelectNodes(@"//div[contains(@class, 'stock-health-check-module-score-description')]");

            for (int i = 0; i < ss1.Count; i++)
            {
                var s = ss1[i].InnerHtml;
            }

            ////*[@id="stock-health-result"]/div[2]
            /////html/body/div[2]/div[3]/div/div[2]/div[2]
            //context.Stocks.Add(stock);
            //await context.SaveChangesAsync();

            //Console.ReadLine();
        }

        static Dictionary<string, dynamic> map = new Dictionary<string, dynamic> {
            { "big5", "big5" },
            { "utf-8", 65001 },
        };

        static HtmlNode GetRootNoteByUrl(string url, bool isUtf8 = true)
        {
            var web = new HtmlWeb();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding webEncode = isUtf8 ? Encoding.GetEncoding(65001) : Encoding.GetEncoding("big5");
            web.OverrideEncoding = webEncode;

            var doc =  web.Load(url);
            return doc.DocumentNode;
        }

        static void ParserFinance(HtmlDocument doc)
        {

            //var node = doc.DocumentNode.SelectSingleNode(@"/html/body/div/table/tbody/tr[2]/td[2]/center/table[1]/tbody/tr/td/table[2]/tbody/tr[2]/td[2]");
            var node = doc.DocumentNode.SelectSingleNode(@"/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[2]");

            var open = node.ChildNodes[3].ChildNodes[3].InnerText;
            var high = node.ChildNodes[3].ChildNodes[7].InnerText;
            var low = node.ChildNodes[3].ChildNodes[11].InnerText;
            var close = node.ChildNodes[3].ChildNodes[15].InnerText;

            //var dateString = "/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[1]/tr[2]/td/div/div[1]/div[2]/div[1]/div[1]/div[1]/span[1]";
            var dateString = "/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[1]/tr[2]/td";

            var dateNode = doc.DocumentNode.SelectSingleNode(dateString);



            var node1 = doc.DocumentNode.SelectSingleNode(@"/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[2]/tr[2]/td[2]");

            var vvv = node.InnerText;
            Console.WriteLine(open);
        }

        static void ParserBasic(HtmlDocument doc) 
        {

            //var node = doc.DocumentNode.SelectSingleNode(@"/html/body/div/table/tbody/tr[2]/td[2]/center/table[1]/tbody/tr/td/table[2]/tbody/tr[2]/td[2]");
            var node = doc.DocumentNode.SelectSingleNode(@"/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[2]");

            var open = node.ChildNodes[3].ChildNodes[3].InnerText;
            var high = node.ChildNodes[3].ChildNodes[7].InnerText;
            var low = node.ChildNodes[3].ChildNodes[11].InnerText;
            var close = node.ChildNodes[3].ChildNodes[15].InnerText;

            //var dateString = "/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[1]/tr[2]/td/div/div[1]/div[2]/div[1]/div[1]/div[1]/span[1]";
            var dateString = "/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[1]/tr[2]/td";

            var dateNode = doc.DocumentNode.SelectSingleNode(dateString);



            var node1 = doc.DocumentNode.SelectSingleNode(@"/html/body/div/table/tr[2]/td[2]/table[1]/tr/td/table[2]/tr[2]/td[2]");

            var vvv = node.InnerText;
            Console.WriteLine(open);
        }
    }
}
