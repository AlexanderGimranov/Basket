using CustomerBasket.Domain.Entities;
using System.Collections.Generic;

namespace CustomerBasket.Domain.DiscountEngine
{
    public interface IGiftProvider
    {
        IEnumerable<Product> GetGiftProducts(Basket basket);
    }
}
