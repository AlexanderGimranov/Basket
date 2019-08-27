using System.Linq;
using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine.GiftCalculator
{
    public class LargeBowlOfTrifleCalculator : IGiftCalculator
    {
        private readonly string _bowlOfTrifleId = "RP-1TB-EITB";
        private readonly string _paperMaskId = "RP-RPM-FITB";
        private readonly string _paperMaskName = "Paper Mask";
        public Product CalculateGift(Basket basket)
        {
            var bowlOfTrifles = basket.Products.Count(i => i.Id == _bowlOfTrifleId);
            if (bowlOfTrifles >= 1)
            {
                return new Product(_paperMaskId, _paperMaskName, 0);
            }
            return null;
        }
    }
}
