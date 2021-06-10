using System;
using System.Collections.Generic;

#nullable disable

namespace PostgresData.Models
{
    public partial class _MonthDatum
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Datetime { get; set; }
        public decimal? 單月月增率 { get; set; }
        public decimal? 單月年增率 { get; set; }
        public decimal? 累積年增率 { get; set; }
        public decimal? ROE { get; set; }
        public decimal? ROA { get; set; }
        public decimal? 董監持股增減 { get; set; }
        public decimal? Close { get; set; }
        public decimal? Percent { get; set; }
        public decimal? 董監持股比例 { get; set; }
        public decimal? 負債總計 { get; set; }
        public decimal? 股東權益 { get; set; }
        public decimal? 股本 { get; set; }
        public decimal? 公告每股淨值 { get; set; }
        public decimal? 毛利率 { get; set; }
        public decimal? 營業利益率 { get; set; }
        public decimal? 每股營業額 { get; set; }
        public decimal? 每股稅後盈餘 { get; set; }
    }
}
