using CustomerBasket.Domain.Entities;

namespace CustomerBasket.Domain.DiscountEngine
{
    public interface IDiscountProvider
    {
        decimal GetDiscountTotal(Basket basket);
    }
}
