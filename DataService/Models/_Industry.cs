using System;
using System.Collections.Generic;

#nullable disable

namespace DataService.Models
{
    public partial class _Industry
    {
        public string Industry { get; set; }
        public int _count { get; set; }
        public int totalCount { get; set; }
        public decimal percent { get; set; }
    }
}
