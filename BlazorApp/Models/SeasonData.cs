using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class SeasonData
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? 資產總計 { get; set; }
        public decimal? 負債總計 { get; set; }
        public decimal? 股東權益 { get; set; }
        public decimal? 股本 { get; set; }
        public decimal? 公告每股淨值 { get; set; }
        public decimal? 毛利率 { get; set; }
        public decimal? 營業利益率 { get; set; }
        public decimal? ROE { get; set; }
        public decimal? ROA { get; set; }
        public decimal? 每股營業額 { get; set; }
        public decimal? 每股稅後盈餘 { get; set; }
    }
}
