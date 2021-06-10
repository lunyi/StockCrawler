using System;
using System.Collections.Generic;

#nullable disable

namespace PostgresData.Models
{
    public partial class RealtimeBestStock
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Datetime { get; set; }
    }
}
