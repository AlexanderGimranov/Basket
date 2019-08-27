using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine
{
    public interface IDiscountCalculator
    {
        decimal CalculateDiscount(Basket basket);
    }
}
