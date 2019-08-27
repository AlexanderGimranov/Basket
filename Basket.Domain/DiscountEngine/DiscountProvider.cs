using System.Collections.Generic;
using System.Linq;
using CustomerBasket.Domain.DiscountEngine.DiscountCalculators;
using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine
{
    public class DiscountProvider : IDiscountProvider
    {
        private readonly IEnumerable<IDiscountCalculator> _discountCalculators;

        public DiscountProvider()
        {
            _discountCalculators = new List<IDiscountCalculator>
            {
                new BagOfPogsDiscountCalculator(),
                new ShurikensDiscountCalculator()
            };
        }
        public decimal GetDiscountTotal(Basket basket)
        {
            var totalDiscount = _discountCalculators.Sum(dc => dc.CalculateDiscount(basket));
            return totalDiscount;
        }
    }
}
