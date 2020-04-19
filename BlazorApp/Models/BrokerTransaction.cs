using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class BrokerTransaction
    {
        public Guid Id { get; set; }
        public string BrokerName { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
        public decimal 買賣超 { get; set; }
        public decimal Percent { get; set; }
    }
}
