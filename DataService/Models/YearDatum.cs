using System;
using System.Collections.Generic;

#nullable disable

namespace DataService.Models
{
    public partial class YearDatum
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public decimal? 資產總計 { get; set; }
        public decimal? 負債總計 { get; set; }
        public decimal? 股東權益 { get; set; }
        public decimal? 公告每股淨值 { get; set; }
        public decimal? 流動負債 { get; set; }
        public decimal? 流動資產 { get; set; }
        public decimal? 每股稅後盈餘 { get; set; }
        public decimal? 稅後純益 { get; set; }
        public decimal? 稅前純益 { get; set; }
        public decimal? 營業毛利 { get; set; }
        public decimal? 毛利率 { get; set; }
        public decimal? 稅後資產報酬率 { get; set; }
        public decimal? 稅後股東權益報酬率 { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
