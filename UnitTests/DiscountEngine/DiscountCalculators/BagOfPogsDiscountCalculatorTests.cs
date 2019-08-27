using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerBasket.Domain.DiscountEngine;
using CustomerBasket.Domain.DiscountEngine.DiscountCalculators;
using CustomerBasket.Domain.Entities;

namespace UnitTests
{
    [TestClass]
    public class BagOfPogsDiscountCalculatorTests
    {
        private readonly string _bagOfPogsId = "RP-25D-SITB";
        [TestMethod]
        public void ShouldReturn0WhenNoBagOfPogs()
        {
            // Arrange
            var basket = new Basket(new DiscountProvider(), new GiftProvider());

            // Sut
            var discountCalculator = new BagOfPogsDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            Assert.IsTrue(discount == 0);
        }

        [TestMethod]
        public void ShouldReturn0WhenOneBagOfPogs()
        {
            // Arrange
            var basket = new Basket(new DiscountProvider(), new GiftProvider());
            basket.AddBasketItem(new Product(_bagOfPogsId, "Bag of Pogs", 1));

            // Sut
            var discountCalculator = new BagOfPogsDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            Assert.IsTrue(discount == 0, "Discount should be 0");
        }

        [TestMethod]
        public void ShouldReturnDiscountWhenTwoBagOfPogs()
        {
            // Arrange
            var bagPrice = 2M;
            var basket = new Basket(new DiscountProvider(), new GiftProvider());
            basket.AddBasketItem(new Product(_bagOfPogsId, "Bag of Pogs", bagPrice));
            basket.AddBasketItem(new Product(_bagOfPogsId, "Bag of Pogs", bagPrice));

            // Sut
            var discountCalculator = new BagOfPogsDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            var expectedDiscount = (bagPrice / 2);
            Assert.IsTrue(discount == expectedDiscount, "Discount returned should be half of price of one bag. Discount: {0} Expected: {1}", discount, expectedDiscount);
        }

        [TestMethod]
        public void ShouldReturnDiscountWhenMoreThenTwoBagOfPogs()
        {
            // Arrange
            var bagPrice = 2M;
            var bags = 4;
            var basket = new Basket(new DiscountProvider(), new GiftProvider());
            for(var i = 0; i < bags; i++)
            {
                basket.AddBasketItem(new Product(_bagOfPogsId, "Bag of Pogs", bagPrice));
            }

            // Sut
            var discountCalculator = new BagOfPogsDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            var expectedDiscount = (bagPrice / 2) * (bags - 1);
            Assert.IsTrue(discount == expectedDiscount, "Discount returned should be half of price of all bags except one bag. Discount: {0} Expected: {1}", discount, expectedDiscount);
        }
    }
}
