using System;
using System.Collections.Generic;
using System.Text;
using DataService.Models;

namespace DataService.DataModel
{
    public class StockeModel
    {
        public Stocks Stock { get; set; }
        public PriceModel[] Prices { get; set; }
    }
    public class PriceModel
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
        public decimal 本益比 { get; set; }
        public int? 融資買賣超 { get; set; }
        public decimal? 融資使用率 { get; set; }

        public int? 外資買賣超 { get; set; }
        public int? 投信買賣超 { get; set; }
        public int? 自營商買賣超 { get; set; }
        public decimal? 外資持股比重 { get; set; }
        public decimal? 三大法人持股比重 { get; set; }
        public int? 融券買進 { get; set; }
        public int? 外資持股 { get; set; }
        public decimal? 外資持股比例 { get; set; }
        public decimal? 主力買賣超 { get; set; }
        public decimal? 籌碼集中度 { get; set; }
        public decimal? 周轉率 { get; set; }
    }
}
