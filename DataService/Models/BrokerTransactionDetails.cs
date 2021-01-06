using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class BrokerTransactionDetails
    {
        public string BrokerId { get; set; }
        public string BrokerName { get; set; }
        public string StockId { get; set; }
        public string StockName { get; set; }
        public DateTime Datetime { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public int 買賣超 { get; set; }
    }
}
