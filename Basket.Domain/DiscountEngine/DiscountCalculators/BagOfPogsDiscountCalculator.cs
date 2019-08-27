using System.Linq;
using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine.DiscountCalculators
{
    public class BagOfPogsDiscountCalculator : IDiscountCalculator
    {
        private readonly string _bagOfPogsId = "RP-25D-SITB";
        public decimal CalculateDiscount(Basket basket)
        {
            var bagsOfPogs = basket.Products.Count(i => i.Id == _bagOfPogsId);
            if (bagsOfPogs > 1)
            {
                var bag = basket.Products.First(bi => bi.Id == _bagOfPogsId);
                var unitPriceDiscount = bag.UnitPrice /= 2;
                var discount = (bagsOfPogs - 1) * unitPriceDiscount;
                return discount;
            }
            return 0;
        }
    }
}
