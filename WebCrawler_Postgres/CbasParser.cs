using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class CbasParser : BaseParser
    {
        public override async Task RunAsync()
        {
            var url = "https://www.tpex.org.tw/web/bond/publish/convertible_bond_search/memo.php?l=zh-tw";
            var rootNode = GetRootNoteByUrl(url);

            var nodes = rootNode.SelectNodes("/html/body/table/tbody/tr");
            var ss = new List<string>();
            for (int i = 0; i < nodes.Count; i++)
            {
                ss.Add(nodes[i].ChildNodes[1].InnerHtml);
            }

            var result = string.Join(',', ss);
            var sw = new StreamWriter("D:\\可轉債.csv", true, Encoding.UTF8);
            await sw.WriteAsync(result);
            sw.Close();
        }
    }
}
