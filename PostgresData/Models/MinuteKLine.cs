using System;
using System.Collections.Generic;

#nullable disable

namespace PostgresData.Models
{
    public partial class MinuteKLine
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}
