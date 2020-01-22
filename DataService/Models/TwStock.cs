using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class TwStock
    {
        public Guid Id { get; set; }
        public DateTime Datetime { get; set; }
        public decimal 收盤價 { get; set; }
        public decimal 成交量 { get; set; }
        public decimal? 漲跌 { get; set; }
        public decimal? 漲跌百分比 { get; set; }
        public decimal 外資買賣超 { get; set; }
        public decimal 投信買賣超 { get; set; }
        public decimal 自營總 { get; set; }
        public decimal 自營自買 { get; set; }
        public decimal 自營避險 { get; set; }
        public decimal 總計 { get; set; }
        public decimal 外資未平倉 { get; set; }
        public decimal 投信未平倉 { get; set; }
        public decimal 自營未平倉 { get; set; }
        public decimal 總計未平倉 { get; set; }
        public decimal 前五大 { get; set; }
        public decimal 前十大 { get; set; }
        public decimal 前五特 { get; set; }
        public decimal 前十特 { get; set; }
        public decimal 融資餘額 { get; set; }
        public decimal 融券餘額 { get; set; }
        public decimal 融資增加 { get; set; }
        public decimal 融券增加 { get; set; }
        public int? 漲停家數 { get; set; }
        public int? 跌停家數 { get; set; }
        public int? 上漲家數 { get; set; }
        public int? 下跌家數 { get; set; }
        public int? 自營選擇權交易口數 { get; set; }
        public int? 自營選擇權未平倉口數 { get; set; }
        public int? 外資選擇權交易口數 { get; set; }
        public int? 外資選擇權未平倉口數 { get; set; }
        public decimal? 交易口數PC比 { get; set; }
        public decimal? 未平倉口數PC比 { get; set; }
    }
}
