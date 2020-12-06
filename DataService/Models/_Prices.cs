using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class _Prices
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Datetime { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal 漲跌 { get; set; }
        public decimal 漲跌百分比 { get; set; }
        public int 成交量 { get; set; }
        public decimal 籌碼集中度 { get; set; }
        public decimal 十日籌碼集中度 { get; set; }
        public decimal 主力買賣比例 { get; set; }
        public decimal? 十日主力買賣比例 { get; set; }
        public decimal? 主力買賣超 { get; set; }
        public decimal? 投信持股比例 { get; set; }
        public int? 董監持股 { get; set; }
        public decimal? 董監持股比例 { get; set; }
        public string DIF { get; set; }
        public string MACD { get; set; }
        public string OSC { get; set; }
        public string RSV { get; set; }
        public string K9 { get; set; }
        public string D9 { get; set; }
        public int? AvgDays { get; set; }
        public string MA5 { get; set; }
        public string MA10 { get; set; }
        public string MA20 { get; set; }
        public string MA60 { get; set; }
        public int? 融資買賣超 { get; set; }
        public int? 融券買賣超 { get; set; }
        public decimal? 當沖張數 { get; set; }
        public decimal? 當沖比例 { get; set; }
        public int? 外資買賣超 { get; set; }
        public int? 投信買賣超 { get; set; }
        public int? 自營商買賣超 { get; set; }
        public decimal 本益比 { get; set; }
        public decimal? 外資持股比例 { get; set; }
        public decimal? 融資使用率 { get; set; }
        public decimal 五日籌碼集中度 { get; set; }
        public decimal 二十日籌碼集中度 { get; set; }
    }
}
