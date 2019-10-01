using System;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Models;
using HtmlAgilityPack;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Update Model
            //Scaffold-DbContext "Server=localhost;Database=StockDb;User ID=sa;Password=sa;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -UseDatabaseNames

            var context = new StockDbContext();

            var stock = new Stocks() {
            };



            string url = "http://5850web.moneydj.com/z/zc/zcx/zcxNew_2498.djhtm";
            var web = new HtmlWeb();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            web.OverrideEncoding = Encoding.GetEncoding("big5");

            var doc = web.Load(url);


            
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



            context.Stocks.Add(stock);
            await context.SaveChangesAsync();

            Console.ReadLine();
        }
    }
}
