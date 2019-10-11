using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Stocks
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public string MarketCategory { get; set; }
        public string Industry { get; set; }
        public DateTimeOffset ListingOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int Status { get; set; }
    }
}
