using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerBasket.Domain.Entities
{
    public class BasketDetails
    {
        public IEnumerable<BasketItem> BasketItems { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Discount { get; set; }

        public decimal GrandTotal { get; set; }
    }
}
