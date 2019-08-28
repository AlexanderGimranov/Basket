using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerBasket.Domain.Entities
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsGift { get; set; }

        public Product(string id, string name, decimal unitPrice)
        {
            Id = id;
            Name = name;
            UnitPrice = unitPrice;
        }
    }
}
