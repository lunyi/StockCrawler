using PostgresData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class CFullyDeliverySharesParser : BaseParser
    {
        public override async Task RunAsync()
        {
            var best = new List<BestStock>();
            
            var url = "https://www.sinotrade.com.tw/Stock/Stock_3_8_3";
            var rootNode = GetRootNoteByUrl(url);

            var nodes = rootNode.SelectNodes("/html/body/div[2]/div[3]/div[2]/table/tbody/tr");
            var ss = new List<string>();
            for (int i = 0; i < nodes.Count; i++)
            {
                ss.Add(nodes[i].ChildNodes[3].InnerHtml);

                best.Add(new BestStock
                {
                    Id = Guid.NewGuid(),
                    Type = "全額交割股",
                    StockId = nodes[i].ChildNodes[3].InnerHtml,
                    Name = nodes[i].ChildNodes[5].InnerHtml,
                    CreatedOn = DateTime.UtcNow
                });
            }

            var context = new stockContext();
            context.BestStocks.AddRange(best);
            await context.SaveChangesAsync();
            var result = string.Join(',', ss);
            var sw = new StreamWriter("D:\\全額交割股.csv", true, Encoding.UTF8);
            await sw.WriteAsync(result);
            sw.Close();
        }
    }
}
