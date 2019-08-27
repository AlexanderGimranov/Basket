using System;
using System.Collections.Generic;
using System.Linq;
using CustomerBasket.Domain.DiscountEngine;

namespace CustomerBasket.Domain.Entities
{
    public class Basket
    {
        private readonly IDiscountProvider _discountProvider;
        private readonly IGiftProvider _giftProvider;

        private readonly List<Product> _products;

        public IReadOnlyCollection<Product> Products => _products;

        public Basket(IDiscountProvider discountProvider, IGiftProvider giftProvider)
        {
            _discountProvider = discountProvider ?? throw new ArgumentNullException("Parameter can't be null", nameof(discountProvider));
            _giftProvider = giftProvider ?? throw new ArgumentNullException("Parameter can't be null", nameof(giftProvider));

            _products = new List<Product>();
        }

        public void AddBasketItem(Product item)
        {
            if (item == null)
                throw new ArgumentNullException("Can't be null", nameof(item));
            if (String.IsNullOrEmpty(item.Id))
                throw new ArgumentNullException("Can't be null", nameof(item.Id));

            _products.Add(item);
            //UpdateGiftItems();
        }

        public void RemoveBasketItem(Product basketItem)
        {
            if(basketItem != null)
            {
                _products.Remove(basketItem);
                //UpdateGiftItems();
            }
        }

        public void RemoveAllItemsOfType(string productId)
        {
            if (!String.IsNullOrEmpty(productId))
            {
                _products.RemoveAll(p => p.Id == productId);
                //UpdateGiftItems();
            }
        }

        public void Empty()
        {
            _products.Clear();
        }

        public decimal Subtotal()
        {
            var subtotal = _products.Sum(i => i.UnitPrice);
            return subtotal;
        }

        public decimal GrandTotal()
        {
            var subtotal = Subtotal();
            var discount = GetDiscount();
            var total = subtotal - discount;
            return total;
        }

        public IEnumerable<BasketItem> GetBasketItems()
        {
            var basketItems = new Dictionary<string, BasketItem>();
            foreach(var product in _products)
            {
                var key = product.Id + product.UnitPrice;
                var basketItemExists = basketItems.TryGetValue(key, out var basketItem);
                if (basketItemExists)
                {
                    basketItem.Units++;
                }
                else
                {
                    basketItem = new BasketItem(product);
                }
                basketItems[key] = basketItem;
            }
            return basketItems.Values;
        }

        public decimal GetDiscount()
        {
            var discount = _discountProvider.GetDiscountTotal(this);
            return discount;
        }

        public BasketDetails GetDetails()
        {
            UpdateGiftItems();
            return new BasketDetails
            {
                BasketItems = GetBasketItems(),
                Subtotal = Subtotal(),
                Discount = GetDiscount(),
                GrandTotal = GrandTotal()
            };
        }

        private void UpdateGiftItems()
        {
            // Remove existing gifts
            // Recalculating of gifts should not add extra products. (Idempotent)
            // Before adding all applicable gifts ensure there are no gifts already in basket
            RemoveGifts();

            var giftItems = _giftProvider.GetGiftProducts(this);
            foreach (var gift in giftItems)
            {
                AddBasketItem(gift);
            }
        }

        private void RemoveGifts()
        {
            _products.RemoveAll(p => p.UnitPrice == 0M);
        }
    }
}
