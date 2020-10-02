using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Broker
    {
        public string BrokerId { get; set; }
        public string BrokerName { get; set; }
        public DateTime? BusinessDay { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string b { get; set; }
    }
}
