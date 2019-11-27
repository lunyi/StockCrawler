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
        public decimal? PercentOverFourHundreds { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? PercentOverThousand { get; set; }
        public decimal? PercentUnderFourHundreds { get; set; }
    }
}
