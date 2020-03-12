using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarehousePulse.Api.Models;
using WarehousePulse.Api.Services;

namespace WarehousePulse.Tests.Services
{
    [TestClass]
    public class StockServiceTests
    {
        private readonly DateTime now = new DateTime(2020, 3, 12, 15, 0, 0, DateTimeKind.Utc);

        [TestMethod]
        public void Apply_ReturnsMovementWithResultingQuantity()
        {
            var item = new InventoryItem { Id = 3, Sku = "NUT-M8", QuantityOnHand = 100 };

            var movement = new StockService().Apply(item, -40, "pick", now);

            Assert.AreEqual(60, item.QuantityOnHand);
            Assert.AreEqual(now, item.UpdatedAt);
            Assert.AreEqual(3, movement.InventoryItemId);
            Assert.AreEqual(-40, movement.Delta);
            Assert.AreEqual(60, movement.QuantityAfter);
        }

        [TestMethod]
        public void Apply_ExactlyToZero_IsFine()
        {
            var item = new InventoryItem { QuantityOnHand = 5 };

            new StockService().Apply(item, -5, null, now);

            Assert.AreEqual(0, item.QuantityOnHand);
        }

        [TestMethod]
        public void Apply_BelowZero_Throws_AndLeavesItemAlone()
        {
            var item = new InventoryItem { Sku = "X", QuantityOnHand = 5 };

            try
            {
                new StockService().Apply(item, -6, null, now);
                Assert.Fail("expected InsufficientStockException");
            }
            catch (InsufficientStockException)
            {
            }

            Assert.AreEqual(5, item.QuantityOnHand);
        }
    }
}
