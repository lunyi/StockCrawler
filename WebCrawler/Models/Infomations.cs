using System;
using System.Collections.Generic;

namespace ConsoleApp.Models
{
    public partial class Infomations
    {
        public string StockId { get; set; }
        public decimal? PriceEarningRatio { get; set; }
        public decimal? EarningsPerShare { get; set; }
        public decimal? PriceBookRatio { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int SerialNumber { get; set; }
        public string Time { get; set; }
    }
}
