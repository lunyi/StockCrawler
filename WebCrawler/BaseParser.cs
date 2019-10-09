using System;
using System.Collections.Generic;
using System.Text;
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
            return web.Load(url).DocumentNode;
        }
    }
}
