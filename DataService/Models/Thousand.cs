using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Thousand
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime? Datetime { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? PercentOver400 { get; set; }
        public decimal? PercentUnder400 { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P5 { get; set; }
        public decimal? P10 { get; set; }
        public decimal? P15 { get; set; }
        public decimal? P20 { get; set; }
        public decimal? P30 { get; set; }
        public decimal? P40 { get; set; }
        public decimal? P50 { get; set; }
        public decimal? P100 { get; set; }
        public decimal? P200 { get; set; }
        public decimal? P400 { get; set; }
        public decimal? P600 { get; set; }
        public decimal? P800 { get; set; }
        public decimal? P1000 { get; set; }
        public decimal? PercentOver1000 { get; set; }
    }
}
