using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
    }
}
