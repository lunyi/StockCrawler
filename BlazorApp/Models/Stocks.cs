using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class Stocks
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public string MarketCategory { get; set; }
        public string Industry { get; set; }
        public DateTimeOffset ListingOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int Status { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string 營收比重 { get; set; }
        public decimal? 股本 { get; set; }
        public decimal? 股價 { get; set; }
        public decimal? 每股淨值 { get; set; }
        public decimal? 每股盈餘 { get; set; }
        public string Description { get; set; }
        public decimal? ROE { get; set; }
        public decimal? ROA { get; set; }
        public bool? 股票期貨 { get; set; }
    }
}
