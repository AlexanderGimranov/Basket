using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerBasket.Domain.DiscountEngine;
using CustomerBasket.Domain.DiscountEngine.DiscountCalculators;
using CustomerBasket.Domain.Entities;

namespace UnitTests
{
    [TestClass]
    public class ShurikensDiscountCalculatorTests
    {
        private readonly string _shurikensId = "RP-5NS-DITB";
        private readonly decimal _shurikensPrice = 20M;
        [TestMethod]
        public void ShouldReturn0WhenNoShurikens()
        {
            // Arrange
            var basket = new Basket(new DiscountProvider(), new GiftProvider());

            // Sut
            var discountCalculator = new ShurikensDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            Assert.IsTrue(discount == 0, "Discount should be 0");
        }

        [TestMethod]
        public void ShouldReturn0WhenShurikensLess100()
        {
            // Arrange
            var basket = new Basket(new DiscountProvider(), new GiftProvider());
            basket.AddBasketItem(new Product(_shurikensId, "Shurikens", _shurikensPrice));

            // Sut
            var discountCalculator = new ShurikensDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            Assert.IsTrue(discount == 0, "Discount should be 0");
        }

        [TestMethod]
        public void ShouldReturnDiscountWhen101Shurikens()
        {
            // Arrange
            var basket = new Basket(new DiscountProvider(), new GiftProvider());
            var shurikensCount = 101;
            for (var i = 0; i < shurikensCount; i++)
            {
                basket.AddBasketItem(new Product(_shurikensId, "Shurikens", _shurikensPrice));
            }

            // Sut
            var discountCalculator = new ShurikensDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            var expectedDiscount = (_shurikensPrice * shurikensCount) * 0.3M;
            Assert.IsTrue(discount == expectedDiscount, "Discount returned should be 30% of total price. Discount: {0} Expected: {1}", discount, expectedDiscount);
        }

        [TestMethod]
        public void ShouldReturnDiscountWhen101ShurikensAndOtherProducts()
        {
            // Arrange
            var basket = new Basket(new DiscountProvider(), new GiftProvider());
            var shurikensCount = 101;
            for (var i = 0; i < shurikensCount; i++)
            {
                basket.AddBasketItem(new Product(_shurikensId, "Shurikens", _shurikensPrice));
            }
            var otherProductPrice = 100;
            basket.AddBasketItem(new Product("Some-Id", "Some Product", otherProductPrice));

            // Sut
            var discountCalculator = new ShurikensDiscountCalculator();

            // Act
            var discount = discountCalculator.CalculateDiscount(basket);

            // Assert
            var expectedDiscount = (_shurikensPrice * shurikensCount + otherProductPrice) * 0.3M;
            Assert.IsTrue(discount == expectedDiscount, "Discount returned should be 30% of total price. Discount: {0} Expected: {1}", discount, expectedDiscount);
        }
    }
}
