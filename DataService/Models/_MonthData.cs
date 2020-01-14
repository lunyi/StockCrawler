using System;
using System.Collections.Generic;

namespace DataService.Models
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
    }
}
