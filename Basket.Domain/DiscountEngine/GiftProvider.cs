using System.Collections.Generic;
using System.Linq;
using CustomerBasket.Domain.DiscountEngine.GiftCalculator;
using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine
{
    public class GiftProvider : IGiftProvider
    {
        private readonly IEnumerable<IGiftCalculator> _giftCalculators;

        public GiftProvider()
        {
            _giftCalculators = new List<IGiftCalculator>
            {
                new LargeBowlOfTrifleCalculator()
            };
        }

        public IEnumerable<Product> GetGiftProducts(Basket basket)
        {
            var gifts = _giftCalculators.Select(c => c.CalculateGift(basket)).Where(g => g != null);
            return gifts;
        }
    }
}
