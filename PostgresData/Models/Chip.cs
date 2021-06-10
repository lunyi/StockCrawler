using System;
using System.Collections.Generic;

#nullable disable

namespace PostgresData.Models
{
    public partial class Chip
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? 主力買進 { get; set; }
        public decimal? 主力賣出 { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
