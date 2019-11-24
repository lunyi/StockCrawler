using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class MonthData
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public decimal 單月營收 { get; set; }
        public decimal 去年同月營收 { get; set; }
        public decimal? 單月月增率 { get; set; }
        public decimal? 單月年增率 { get; set; }
        public decimal? 累計營收 { get; set; }
        public decimal? 去年累計營收 { get; set; }
        public decimal? 累積年增率 { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
