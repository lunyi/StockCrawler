using System;
using System.Collections.Generic;

namespace BlazorApp.Models
{
    public partial class AnaStatementDogs
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Pass { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
