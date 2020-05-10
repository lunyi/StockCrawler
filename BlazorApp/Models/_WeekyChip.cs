﻿using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class _WeekyChip
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Datetime { get; set; }
        public decimal? Close { get; set; }
        public decimal PUnder100 { get; set; }
        public decimal PUnder400 { get; set; }
        public decimal POver400 { get; set; }
        public decimal POver1000 { get; set; }
        public int SUnder100 { get; set; }
        public int SUnder400 { get; set; }
        public int SOver400 { get; set; }
        public int SOver1000 { get; set; }
    }
}