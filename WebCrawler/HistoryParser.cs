using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataService.Models;

namespace WebCrawler
{
    public class HistoryParser : BaseParser
    {
        //https://www.cnyes.com/twstock/ps_historyprice/2330.htm
        //法人進出
        //http://5850web.moneydj.com/z/zc/zcl/zcl.djhtm?a=1101&c=2016-01-27&d=2019-10-25
        //主力進出
        //http://5850web.moneydj.com/z/zc/zco/zco.djhtm?a=1101&e=2014-10-22&f=2014-10-22
        //券商分點進出明細
        //http://5850web.moneydj.com/z/zc/zco/zco0/zco0.djhtm?A=1101&BHID=1380&b=1380&C=1&D=2019-9-23&E=2019-10-25&ver=V3
        //http://5850web.moneydj.com/z/zc/zco/zco0/zco0.djhtm?A=1102&BHID=1020&b=1020&C=1&D=2019-9-20&E=2019-10-25&ver=V3
        //融資
        //http://5850web.moneydj.com/z/zc/zcn/zcn.djhtm?a=1102&c=2017-1-1&d=2019-10-25

        public (decimal, decimal) ParseMainForce(string stockId, string startDate, string endDate)
        {
            var url = $"https://concords.moneydj.com/z/zc/zco/zco.djhtm?a={stockId}&e={startDate}&f={endDate}";
            var rootNode = GetRootNoteByUrl(url, false);
            var nodes = rootNode.SelectNodes("/html[1]/body[1]/div[1]/table[1]/tr[2]/td[2]/form[1]/table[1]/tr[1]/td[1]/table[1]/tr");

            decimal 主力買超張數 = 0, 主力賣超張數 = 0;

            for (int i = 6; i < nodes.Count; i++)
            {
                var node = nodes[i];

                if (node.ChildNodes[1].InnerHtml == "合計買超張數")
                {
                    主力買超張數 = Convert.ToDecimal(node.ChildNodes[3].InnerHtml.Replace(",", ""));
                    主力賣超張數 = Convert.ToDecimal(node.ChildNodes[7].InnerHtml.Replace(",", ""));

                }
                else if (node.ChildNodes[1].InnerHtml == "合計買超股數")
                {
                    主力買超張數 = Convert.ToDecimal(node.ChildNodes[3].InnerHtml.Replace(",", "")) / 1000;
                    主力賣超張數 = Convert.ToDecimal(node.ChildNodes[7].InnerHtml.Replace(",", "")) / 1000;
                }
            }

            return (主力買超張數, 主力賣超張數);
        }

        public HistoryPrice[] TrustParser(string stockId, string startDate, string endDate)
        {
            var url = $"http://5850web.moneydj.com/z/zc/zcl/zcl.djhtm?a={stockId}&c={startDate}&d={endDate}";
            var rootNode = GetRootNoteByUrl(url, false);
            var node = rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/form/table/tr/td/table");

            var prices = new List<HistoryPrice>();
            for (int i = 15; i < node.ChildNodes.Count - 2; i += 2)
            {
                var c = node.ChildNodes[i];
                var s = new HistoryPrice
                {
                    Datetime = Convert.ToDateTime(node.ChildNodes[i].ChildNodes[1].InnerText).AddYears(1911),
                    外資買賣超 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[3].InnerText.Replace(",", "")),
                    投信買賣超 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[5].InnerText.Replace(",", "")),
                    自營商買賣超 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[7].InnerText.Replace(",", "")),
                    外資持股 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[9].InnerText.Replace(",", "")),
                    投信持股 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[13].InnerText.Replace(",", "")),
                    自營商持股 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[15].InnerText.Replace(",", "")),
                    外資持股比重 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[19].InnerText.Replace(",", "").Replace("%", "")),
                    三大法人持股比重 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[19].InnerText.Replace(",", "").Replace("%", "")),
                };
                prices.Add(s);
            }
            return prices.ToArray();
        }

        public HistoryPrice[] FinancingParser(string stockId, string startDate, string endDate)
        {
            var url = $"http://5850web.moneydj.com/z/zc/zcn/zcn.djhtm?a={stockId}&c={startDate}&d={endDate}";
            var rootNode = GetRootNoteByUrl(url, false);
            var node = rootNode.SelectSingleNode("//*[@id='SysJustIFRAMEDIV']/table/tr[2]/td[2]/form/table/tr/td/table");

            var prices = new List<HistoryPrice>();
            for (int i = 15; i < node.ChildNodes.Count - 2; i+=2)
            {
                var c = node.ChildNodes[i];
                var s = new HistoryPrice
                {
                    Datetime = Convert.ToDateTime(node.ChildNodes[i].ChildNodes[1].InnerText).AddYears(1911),
                    融資買進 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[3].InnerText.Replace(",","")),
                    融資賣出 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[5].InnerText.Replace(",", "")),
                    融資現償 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[7].InnerText.Replace(",", "")),
                    融資餘額 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[9].InnerText.Replace(",", "")),
                    融資限額 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[13].InnerText.Replace(",", "")),
                    融資使用率 = Convert.ToDecimal(node.ChildNodes[i].ChildNodes[15].InnerText.Replace(",", "").Replace("%", "")),
                    融券買進 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[19].InnerText.Replace(",", "")),
                    融券賣出 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[17].InnerText.Replace(",", "")),
                    融券餘額 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[23].InnerText.Replace(",", "")),
                    券資比 = Convert.ToDecimal(node.ChildNodes[i].ChildNodes[27].InnerText.Replace(",", "").Replace("%", "")),
                    資券相抵 = Convert.ToInt32(node.ChildNodes[i].ChildNodes[29].InnerText.Replace(",", "")),
                };
                prices.Add(s);
            }
            return prices.ToArray();
        }
    }
}
