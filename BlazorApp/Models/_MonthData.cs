using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class _MonthData
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
    }
}
