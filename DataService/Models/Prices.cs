using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Prices
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int? 融資買進 { get; set; }
        public int? 融資賣出 { get; set; }
        public int? 融資現償 { get; set; }
        public int? 融資餘額 { get; set; }
        public int? 融資限額 { get; set; }
        public decimal? 融資使用率 { get; set; }
        public int? 融券買進 { get; set; }
        public int? 融券賣出 { get; set; }
        public int? 融券券償 { get; set; }
        public int? 融券餘額 { get; set; }
        public int? 券資比 { get; set; }
        public int? 資券相抵 { get; set; }
        public int? 外資持股 { get; set; }
        public decimal? 外資持股比例 { get; set; }
        public int? 外資買賣超 { get; set; }
        public int? 投信持股 { get; set; }
        public int? 投信買賣超 { get; set; }
    }
}
