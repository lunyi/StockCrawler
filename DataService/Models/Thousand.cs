using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Thousand
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal? PPUnder100 { get; set; }
        public decimal PUnder100 { get; set; }
        public decimal P1 { get; set; }
        public decimal P5 { get; set; }
        public decimal P10 { get; set; }
        public decimal P15 { get; set; }
        public decimal P20 { get; set; }
        public decimal P30 { get; set; }
        public decimal P40 { get; set; }
        public decimal P50 { get; set; }
        public decimal P100 { get; set; }
        public decimal P200 { get; set; }
        public decimal P400 { get; set; }
        public decimal P600 { get; set; }
        public decimal P800 { get; set; }
        public decimal P1000 { get; set; }
        public decimal? PPOver1000 { get; set; }
        public decimal POver1000 { get; set; }
        public int C1 { get; set; }
        public int C5 { get; set; }
        public int C10 { get; set; }
        public int C15 { get; set; }
        public int C20 { get; set; }
        public int C30 { get; set; }
        public int C40 { get; set; }
        public int C50 { get; set; }
        public int C100 { get; set; }
        public int C200 { get; set; }
        public int C400 { get; set; }
        public int C600 { get; set; }
        public int C800 { get; set; }
        public int C1000 { get; set; }
        public int COver1000 { get; set; }
        public decimal S1 { get; set; }
        public decimal S5 { get; set; }
        public decimal S10 { get; set; }
        public decimal S15 { get; set; }
        public decimal S20 { get; set; }
        public decimal S30 { get; set; }
        public decimal S40 { get; set; }
        public decimal S50 { get; set; }
        public decimal S100 { get; set; }
        public decimal S200 { get; set; }
        public decimal S400 { get; set; }
        public decimal S600 { get; set; }
        public decimal S800 { get; set; }
        public decimal S1000 { get; set; }
        public decimal SOver1000 { get; set; }
        public int? TotalCount { get; set; }
        public decimal? TotalStock { get; set; }
        public int? CUnder100 { get; set; }
        public decimal? SUnder100 { get; set; }
    }
}
