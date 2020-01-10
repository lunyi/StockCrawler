using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class _WeekyChip
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Datetime { get; set; }
        public decimal? Close { get; set; }
        public decimal PUnder100 { get; set; }
        public decimal PUnder400 { get; set; }
        public decimal POver400 { get; set; }
        public decimal POver1000 { get; set; }
        public int SUnder100 { get; set; }
        public int SUnder400 { get; set; }
        public int SOver400 { get; set; }
        public int SOver1000 { get; set; }
        public int? 外資買賣超 { get; set; }
        public int? 融資買賣超 { get; set; }
        public int? 投信買賣超 { get; set; }
        public int? 自營商買賣超 { get; set; }
        public int? 主力買賣超 { get; set; }
        public int? 董監持股 { get; set; }
        public int? 成交量 { get; set; }
    }
}
