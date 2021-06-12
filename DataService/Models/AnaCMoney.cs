using System;
using System.Collections.Generic;

#nullable disable

namespace DataService.Models
{
    public partial class AnaCMoney
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? 價值 { get; set; }
        public int? 安全 { get; set; }
        public int? 成長 { get; set; }
        public int? 籌碼 { get; set; }
        public int? 技術 { get; set; }
    }
}
