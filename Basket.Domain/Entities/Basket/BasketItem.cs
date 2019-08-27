namespace CustomerBasket.Domain.Entities
{
    public class BasketItem
    {
        public Product Product { get; set; }

        public int Units { get; set; }

        public decimal Subtotal
        {
            get
            {
                return Product.UnitPrice * Units;
            }
        }

        public BasketItem(Product product)
        {
            Product = product;
            Units = 1;
        }
    }
}
