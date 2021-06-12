using System;
using System.Collections.Generic;

#nullable disable

namespace DataService.Models
{
    public partial class StockBroker
    {
        public Guid Id { get; set; }
        public string BrokerId { get; set; }
        public string BrokerName { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Volume { get; set; }
        public int? Count { get; set; }
    }
}
