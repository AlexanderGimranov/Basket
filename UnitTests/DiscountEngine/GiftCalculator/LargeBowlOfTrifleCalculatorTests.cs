using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerBasket.Domain.DiscountEngine;
using CustomerBasket.Domain.DiscountEngine.GiftCalculator;
using CustomerBasket.Domain.Entities;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class LargeBowlOfTrifleCalculatorTests
    {
        private readonly string _bowlOfTrifleId = "RP-1TB-EITB";
        private readonly string _paperMaskId = "RP-RPM-FITB";
        private readonly decimal _bowlOfTriflePrice = 20M;

        [TestMethod]
        public void ShouldReturnNoGiftsWhenNoBowls()
        {
            // Arrange
            var basket = new Basket(new DummyDiscountProvider(), new DummyGiftProvider());

            // Sut
            var giftCalculator = new LargeBowlOfTrifleCalculator();

            // Act
            var gift = giftCalculator.CalculateGift(basket);

            // Assert
            Assert.IsNull(gift);
        }

        [TestMethod]
        public void ShouldReturnPaperMaskWhenBowlIsInBasket()
        {
            // Arrange
            var basket = new Basket(new DummyDiscountProvider(), new DummyGiftProvider());
            basket.AddBasketItem(new Product(_bowlOfTrifleId, "Bowl Of Trifle", _bowlOfTriflePrice));

            // Sut
            var giftCalculator = new LargeBowlOfTrifleCalculator();

            // Act
            var gift = giftCalculator.CalculateGift(basket);

            // Assert
            Assert.IsNotNull(gift);
            Assert.IsTrue(gift.Id == _paperMaskId, "Product Id should be Paper Mask Id");
            Assert.AreEqual(gift.UnitPrice, 0, "Product Price should be 0");
        }
    }

    internal class DummyGiftProvider : IGiftProvider
    {
        public IEnumerable<Product> GetGiftProducts(Basket basket)
        {
            return new List<Product>();
        }
    }

    internal class DummyDiscountProvider : IDiscountProvider
    {
        public decimal GetDiscountTotal(Basket basket)
        {
            throw new System.NotImplementedException();
        }
    }
}
