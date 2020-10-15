using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class _MinuteKLine
    {
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime? Datetime { get; set; }
    }
}
