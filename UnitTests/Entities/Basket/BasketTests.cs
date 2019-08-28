using CustomerBasket.Domain.DiscountEngine;
using CustomerBasket.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Entities.CustomerBasket
{
    [TestClass]
    public class BasketTests
    {
        private readonly Mock<IDiscountProvider> _discountProvider;
        private readonly Mock<IGiftProvider> _giftProvider;

        public BasketTests()
        {
            _discountProvider = new Mock<IDiscountProvider>(MockBehavior.Loose);
            _giftProvider = new Mock<IGiftProvider>(MockBehavior.Loose);
        }

        [TestMethod]
        public void ShouldThrowExceptionIfDiscountProviderNull()
        {
            // Act, Assert
            Assert.ThrowsException<ArgumentNullException>(() => new Basket(null, _giftProvider.Object));
        }

        [TestMethod]
        public void ShouldThrowExceptionIfGiftProviderNull()
        {
            // Act, Assert
            Assert.ThrowsException<ArgumentNullException>(() => new Basket(_discountProvider.Object, null));
        }

        [TestMethod]
        public void ShouldCreateInstanse()
        {
            // Act
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Assert
            Assert.IsNotNull(basket);
        }

        [TestMethod]
        public void ShouldAddBasketItem()
        {
            // Arrange
            var productId = "Some-Id";
            var product = new Product(productId, "Test", 23);

            // Sut
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act
            basket.AddBasketItem(product);

            // Assert
            Assert.AreEqual(1, basket.Products.Count, "Should have one product");
            Assert.AreEqual(productId, basket.Products.First().Id, $"Ids are not equal, Should be {productId}");
        }

        public void ShouldAddSeveralBasketItems()
        {
            // Arrange
            var product = CreateTestProduct();

            // Sut
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act
            basket.AddBasketItem(product);
            basket.AddBasketItem(product);
            basket.AddBasketItem(product);

            // Assert
            Assert.AreEqual(3, basket.Products.Count, "Should have 3 products");
        }

        [TestMethod]
        public void ShouldThrowWhenAddInvalidBasketItem()
        {
            // Arrange
            var product = new Product(null, "Test", 23);

            // Sut
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act, Assert
            Assert.ThrowsException<ArgumentNullException>(() => basket.AddBasketItem(product));
        }

        [TestMethod]
        public void ShouldThrowWhenAddNull()
        {
            // Sut
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act, Assert
            Assert.ThrowsException<ArgumentNullException>(() => basket.AddBasketItem(null));
        }

        [TestMethod]
        public void ShouldCallGiftProviderAndAddGiftWhenAddItem()
        {
            // Arrange
            var productId = "Some-Id";
            var product = new Product(productId, "Test", 23);
            var gift = CreateTestProduct("gift");
            var gifts = new List<Product> { gift };
            _giftProvider.Setup(p => p.GetGiftProducts(It.IsAny<Basket>())).Returns(gifts);

            // Sut
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act
            basket.AddBasketItem(product);

            // Assert
            _giftProvider.VerifyAll();
            var items = basket.Products;
            var expectedProductCount = 2; // product + gift
            Assert.AreEqual(expectedProductCount, items.Count, $"Amount of products in basket is not as expected. Expected: {expectedProductCount}, Actual: {items.Count}");
        }

        [TestMethod]
        public void ShouldRemoveOneBasketItem()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            basket.RemoveBasketItem(product);

            // Assert
            Assert.AreEqual(productCount - 1, basket.Products.Count, $"Should have {productCount - 1} products");
        }

        [TestMethod]
        public void ShouldRemoveNothing()
        {
            // Arrange
            var product = CreateTestProduct();
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act
            basket.RemoveBasketItem(product);

            // Assert
            Assert.AreEqual(0, basket.Products.Count, "Should have 0 product");
        }

        [TestMethod]
        public void ShouldRemoveOneBasketItemAndUpdateGifts()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;
            var gift = CreateTestProduct("gift");
            gift.IsGift = true;
            var noGifts = new List<Product>();
            _giftProvider.Setup(i => i.GetGiftProducts(It.IsAny<Basket>())).Returns(noGifts);
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, gift, productCount);
            Assert.AreEqual(productCount + productCount, basket.Products.Count, $"Should have {productCount - 1} products and gifts in total");

            // Act
            basket.RemoveBasketItem(product);

            // Assert
            Assert.AreEqual(productCount - 1, basket.Products.Count, $"Should have {productCount - 1} products and 0 gifts");
        }

        [TestMethod]
        public void ShouldRemoveAllWithId()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            basket.RemoveAllItemsOfType(product.Id);

            // Assert
            Assert.AreEqual(0, basket.Products.Count, "Should be empty");
        }

        [TestMethod]
        public void ShouldRemoveAllWithSpecifiedIdButKeepOther()
        {
            // Arrange
            var productId = "Some-Id";
            var otherProductId = "Test";
            var product = CreateTestProduct(id: productId);
            var otherProduct = CreateTestProduct(id: otherProductId);
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, otherProduct, 1);

            // Act
            basket.RemoveAllItemsOfType(productId);

            // Assert
            Assert.AreEqual(1, basket.Products.Count, "Should have 1 product");
        }

        [TestMethod]
        public void ShouldNotRemoveAnythingWhenIdNull()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            basket.RemoveAllItemsOfType(null);

            // Assert
            Assert.AreEqual(productCount, basket.Products.Count, $"Should still have {productCount} products");
        }

        [TestMethod]
        public void ShouldNotRemoveAnythingWhenIdDoesNotMatch()
        {
            // Arrange
            var product = CreateTestProduct();
            var otherId = "Other-Id";
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            basket.RemoveAllItemsOfType(otherId);

            // Assert
            Assert.AreEqual(productCount, basket.Products.Count, $"Should still have {productCount} products");
        }

        [TestMethod]
        public void ShouldRemoveAllWithIdAndUpdateGifts()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;
            var giftCount = 1;
            var gift = CreateTestProduct("gift");
            gift.IsGift = true;
            var noGifts = new List<Product>();
            _giftProvider.Setup(i => i.GetGiftProducts(It.IsAny<Basket>())).Returns(noGifts);
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, gift, giftCount);
            Assert.AreEqual(productCount + giftCount, basket.Products.Count, $"Should have {productCount} products and {giftCount} gifts");

            // Act
            basket.RemoveAllItemsOfType(product.Id);

            // Assert
            Assert.AreEqual(0, basket.Products.Count, "Should be empty");
        }

        [TestMethod]
        public void ShouldClearBasket()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            basket.Empty();

            // Assert
            Assert.AreEqual(0, basket.Products.Count, "Should have no products");
        }

        [TestMethod]
        public void ShouldCalculateSubtotalForOneProductl()
        {
            // Arrange
            var productPrice = 23;
            var product = CreateTestProduct(price:productPrice);
            var productCount = 1;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            var subtotal = basket.Subtotal();

            // Assert
            var expectedPrice = productPrice * productCount;
            Assert.AreEqual(expectedPrice, subtotal, $"Subtotal should be {expectedPrice}");
        }

        [TestMethod]
        public void ShouldCalculateSubtotalForMultipleProducts()
        {
            // Arrange
            var productPrice = 23;
            var product = CreateTestProduct(price:productPrice);
            var productCount = 5;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            var subtotal = basket.Subtotal();

            // Assert
            var expectedPrice = productPrice * productCount;
            Assert.AreEqual(expectedPrice, subtotal, $"Subtotal should be {expectedPrice}");
        }

        [TestMethod]
        public void ShouldCalculateSubtotalForDifferentProducts()
        {
            // Arrange
            var productPrice = 23;
            var otherProductId = "Some-Id-2";
            var otherProductPrice = 2;
            var product = CreateTestProduct(price: productPrice);
            var otherProduct = CreateTestProduct(id: otherProductId, price: otherProductPrice);
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, otherProduct, productCount);

            // Act
            var subtotal = basket.Subtotal();

            // Assert
            var expectedPrice = productPrice * productCount + otherProductPrice * productCount;
            Assert.AreEqual(expectedPrice, subtotal, $"Subtotal should be {expectedPrice}");
        }

        [TestMethod]
        public void ShouldCalculateSubtotalForDifferentProductsWithGifts()
        {
            // Arrange
            var productPrice = 23;
            var otherProductId = "Some-Id-2";
            var giftId = "gift";
            var otherProductPrice = 2;
            var giftCount = 2;
            var giftPrice = 200M;
            
            var product = CreateTestProduct(price: productPrice);
            var otherProduct = CreateTestProduct(id: otherProductId, price: otherProductPrice);
            var gift = CreateTestProduct(id: giftId, price: giftPrice);
            gift.IsGift = true;
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, otherProduct, productCount);
            PopulateWithProducts(basket, gift, giftCount);

            // Act
            var subtotal = basket.Subtotal();

            // Assert
            var expectedProductCount = productCount + productCount + giftCount;
            Assert.AreEqual(expectedProductCount, basket.Products.Count, $"Basket should have {expectedProductCount} products including gifts");
            var expectedPrice = productPrice * productCount + otherProductPrice * productCount;
            Assert.AreEqual(expectedPrice, subtotal, $"Subtotal should be {expectedPrice}");
        }

        [TestMethod]
        public void ShouldGetBasketItemsForDifferentProducts()
        {
            // Arrange
            var productId = "Some-Id";
            var productPrice = 23;
            var otherProductId = "Some-Id-2";
            var otherProductPrice = 2;
            var product = CreateTestProduct(id: productId, price: productPrice);
            var otherProduct = CreateTestProduct(id: otherProductId, price: otherProductPrice);
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, otherProduct, productCount);

            // Act
            var items = basket.GetBasketItems();

            // Assert
            Assert.AreEqual(2, items.Count(), $"Should be 2 types of product");

            var firstBasketItem = items.First(i => i.Product.Id == productId);
            var firstBasketItemSubtotal = productPrice * productCount;
            Assert.AreEqual(productCount, firstBasketItem.Units, $"Should have {productCount} units of product");
            Assert.AreEqual(firstBasketItemSubtotal, firstBasketItem.Subtotal, $"Subtotal should be {firstBasketItemSubtotal}");

            var othertBasketItem = items.First(i => i.Product.Id == otherProductId);
            var secondBasketItemSubtotal = otherProductPrice * productCount;
            Assert.AreEqual(productCount, othertBasketItem.Units, $"Should have {productCount} units of product");
            Assert.AreEqual(secondBasketItemSubtotal, othertBasketItem.Subtotal, $"Subtotal should be {secondBasketItemSubtotal}");
        }

        [TestMethod]
        public void ShouldGetBasketItemsForOneProduct()
        {
            // Arrange
            var productId = "Some-Id";
            var productPrice = 23;
            var product = CreateTestProduct(id: productId, price: productPrice);
            var productCount = 1;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            var items = basket.GetBasketItems();

            // Assert
            Assert.AreEqual(1, items.Count(), $"Should be one type of product");
            var basketItem = items.First();
            Assert.AreEqual(productId, basketItem.Product.Id, $"Should be one type of product");
            Assert.AreEqual(productCount, basketItem.Units, $"Should have 1 unit of product");
            var subtotal = productCount * productPrice;
            Assert.AreEqual(subtotal, basketItem.Subtotal, $"Subtotal should be {subtotal}");
        }

        [TestMethod]
        public void ShouldGetBasketItemsForSeveralUnitsOfOneProduct()
        {
            // Arrange
            var productId = "Some-Id";
            var productPrice = 23;
            var product = CreateTestProduct(id: productId, price: productPrice);
            var productCount = 3;
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            var items = basket.GetBasketItems();

            // Assert
            Assert.AreEqual(1, items.Count(), $"Should be one type of product");
            var basketItem = items.First();
            Assert.AreEqual(productId, basketItem.Product.Id, $"Should be one type of product");
            Assert.AreEqual(productCount, basketItem.Units, $"Should have {productCount} units of product");
            var subtotal = productCount * productPrice;
            Assert.AreEqual(subtotal, basketItem.Subtotal, $"Subtotal should be {subtotal}");
        }

        [TestMethod]
        public void ShouldGetBasketItemsAndReturnNothingForEmptyCollection()
        {
            // Arrange
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            // Act
            var items = basket.GetBasketItems();

            // Assert
            Assert.AreEqual(0, items.Count(), $"Should be no products");
        }

        [TestMethod]
        public void ShouldCalculateGrandTotalThatIsSubtotal()
        {
            // Arrange
            var productPrice = 23;
            var product = CreateTestProduct(price: productPrice);
            var productCount = 3;
            _discountProvider.Setup(p => p.GetDiscountTotal(It.IsAny<Basket>())).Returns(0);
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);


            // Act
            var total = basket.GrandTotal();

            // Assert
            var expectedTotal = productCount * productPrice;
            Assert.AreEqual(expectedTotal, total, $"Should calculate total as {expectedTotal}");
        }

        [TestMethod]
        public void ShouldCalculateGrandTotalThatIsSubtotalForDifferentProducts()
        {
            // Arrange
            var productId = "Some-Id";
            var productPrice = 23;
            var otherProductId = "Some-Id-2";
            var otherProductPrice = 2;
            var product = CreateTestProduct(id: productId, price: productPrice);
            var otherProduct = CreateTestProduct(id: otherProductId, price: otherProductPrice);
            var productCount = 3;
            var otherProductCount = 3;
            _discountProvider.Setup(p => p.GetDiscountTotal(It.IsAny<Basket>())).Returns(0);
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);
            PopulateWithProducts(basket, otherProduct, otherProductCount);


            // Act
            var total = basket.GrandTotal();

            // Assert
            var expectedTotal = productCount * productPrice + otherProductCount * otherProductPrice;
            Assert.AreEqual(expectedTotal, total, $"Should calculate total as {expectedTotal}");
        }

        [TestMethod]
        public void ShouldCalculateGrandTotalIncludingDiscount()
        {
            // Arrange
            var productPrice = 23;
            var product = CreateTestProduct(price: productPrice);
            var productCount = 3;
            var discount = 15.5M;

            _discountProvider.Setup(p => p.GetDiscountTotal(It.IsAny<Basket>())).Returns(discount);
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            var total = basket.GrandTotal();

            // Assert
            var expectedTotal = productCount * productPrice - discount;
            Assert.AreEqual(expectedTotal, total, $"Should calculate total as {expectedTotal}");
        }

        [TestMethod]
        public void ShouldAddGift()
        {
            // Arrange
            var product = CreateTestProduct();
            var productCount = 3;

            var giftId = "gift";
            var giftName = "Gift";
            var giftPrice = 0;
            var gift = new Product(giftId, giftName, giftPrice);

            _giftProvider.Setup(p => p.GetGiftProducts(It.IsAny<Basket>())).Returns(new List<Product> { gift });
            _discountProvider.Setup(p => p.GetDiscountTotal(It.IsAny<Basket>())).Returns(0);
            var basket = new Basket(_discountProvider.Object, _giftProvider.Object);

            PopulateWithProducts(basket, product, productCount);

            // Act
            var details = basket.GetDetails();

            // Assert
            var expectedTotal = productCount * product.UnitPrice;
            Assert.AreEqual(expectedTotal, details.GrandTotal, $"Should calculate total as {expectedTotal}");
            Assert.AreEqual(2, details.BasketItems.Count(), $"Should be {2} products");
            var giftItem = details.BasketItems.FirstOrDefault(i => i.Product.Id == giftId);
            Assert.IsNotNull(giftItem);
            Assert.AreEqual(giftName, giftItem.Product.Name, "Gift item name does not correspond to expected name");
            Assert.AreEqual(0, giftItem.Subtotal, "Gift price should be 0");
            Assert.AreEqual(1, giftItem.Units, "Gift units should be 1");
        }

        private void PopulateWithProducts(Basket basket, Product product, int count)
        {
            for (int i = 0; i < count; i++)
            {
                basket.AddBasketItem(product);
            }
        }

        private Product CreateTestProduct(string id = "test", string name = "Test", decimal price = 20)
        {
            return new Product(id, name, price);
        }
    }
}
