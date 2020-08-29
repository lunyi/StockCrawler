using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class BaseParser
    {
        private Dictionary<string, dynamic> map = new Dictionary<string, dynamic> 
        {
            { "big5", "big5" },
            { "utf-8", 65001 },
        };

        protected HtmlNode GetRootNoteByUrl(string url, bool isUtf8 = true)
        {
            var web = new HtmlWeb();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            web.OverrideEncoding = isUtf8 ? Encoding.GetEncoding(65001) : Encoding.GetEncoding("big5");
            var res = web.Load(url).DocumentNode;
            return res;
        }

        protected void Log(string message)
        {
            StreamWriter w = new StreamWriter($"D:\\Code\\{DateTime.Today.ToString("yyyy-MM-dd")}.txt", true, Encoding.UTF8);
            w.WriteLine(message);
            w.Close();
        }
    }
}
