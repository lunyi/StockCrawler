using System;
using System.Collections.Generic;

#nullable disable

namespace DataService.Models
{
    public partial class _MinuteKLine
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime? Datetime { get; set; }
    }
}
