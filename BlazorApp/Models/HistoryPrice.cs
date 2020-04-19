using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class HistoryPrice
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal 漲跌 { get; set; }
        public decimal 漲跌百分比 { get; set; }
        public int 成交量 { get; set; }
        public int 成交金額 { get; set; }
        public decimal 本益比 { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? 外資買賣超 { get; set; }
        public int? 投信買賣超 { get; set; }
        public int? 自營商買賣超 { get; set; }
        public int? 外資持股 { get; set; }
        public int? 投信持股 { get; set; }
        public int? 自營商持股 { get; set; }
        public decimal? 外資持股比重 { get; set; }
        public decimal? 三大法人持股比重 { get; set; }
        public decimal? 主力買超張數 { get; set; }
        public decimal? 主力賣超張數 { get; set; }
        public int? 融資買進 { get; set; }
        public int? 融資賣出 { get; set; }
        public int? 融資現償 { get; set; }
        public int? 融資餘額 { get; set; }
        public int? 融資限額 { get; set; }
        public decimal? 融資使用率 { get; set; }
        public int? 融券買進 { get; set; }
        public int? 融券賣出 { get; set; }
        public int? 融券餘額 { get; set; }
        public int? 資券相抵 { get; set; }
        public decimal? 券資比 { get; set; }
        public decimal? 五日主力買超張數 { get; set; }
        public decimal? 五日主力賣超張數 { get; set; }
        public decimal? 十日主力買超張數 { get; set; }
        public decimal? 十日主力賣超張數 { get; set; }
        public decimal? 二十日主力買超張數 { get; set; }
        public decimal? 二十日主力賣超張數 { get; set; }
        public decimal? 四十日主力買超張數 { get; set; }
        public decimal? 四十日主力賣超張數 { get; set; }
        public decimal? 六十日主力買超張數 { get; set; }
        public decimal? 六十日主力賣超張數 { get; set; }
    }
}
