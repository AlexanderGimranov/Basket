using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine.GiftCalculator
{
    public interface IGiftCalculator
    {
        Product CalculateGift(Basket basket);
    }
}
