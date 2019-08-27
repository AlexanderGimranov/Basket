using System.Linq;
using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine.DiscountCalculators
{
    public class ShurikensDiscountCalculator : IDiscountCalculator
    {
        private readonly string _shurikensId = "RP-5NS-DITB";
        public decimal CalculateDiscount(Basket basket)
        {
            var shurikens = basket.Products.Count(i => i.Id == _shurikensId);
            if (shurikens > 100)
            {
                var basketTotal = basket.Subtotal();
                var discount = basketTotal * 0.3M;
                return discount;
            }
            return 0;
        }
    }
}
