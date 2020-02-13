using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WarehousePulse.Api.Controllers;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;
using WarehousePulse.Api.Models.Dto;

namespace WarehousePulse.Tests.Controllers
{
    [TestClass]
    public class InventoryItemsControllerTests
    {
        private Mock<IInventoryRepository> repo;
        private InventoryItem item;
        private List<StockMovement> movements;

        [TestInitialize]
        public void Setup()
        {
            item = new InventoryItem { Id = 7, Sku = "BOX-S-01", Name = "Box", QuantityOnHand = 10, UnitOfMeasure = "ea" };
            movements = new List<StockMovement>();

            repo = new Mock<IInventoryRepository>();
            repo.Setup(r => r.GetAsync(7)).ReturnsAsync(item);
            repo.Setup(r => r.AddMovement(It.IsAny<StockMovement>())).Callback<StockMovement>(m => movements.Add(m));
            repo.Setup(r => r.SaveAsync()).Returns(Task.FromResult(0));
        }

        [TestMethod]
        public async Task Adjust_AddsToQuantity_AndRecordsMovement()
        {
            var controller = new InventoryItemsController(repo.Object);

            var result = await controller.Adjust(7, new AdjustStockRequest { Delta = 5, Reason = "PO 1001" });

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<InventoryItemDto>));
            Assert.AreEqual(15, item.QuantityOnHand);
            Assert.AreEqual(1, movements.Count);
            Assert.AreEqual(5, movements[0].Delta);
            Assert.AreEqual(15, movements[0].QuantityAfter);
        }

        [TestMethod]
        public async Task Adjust_CantGoBelowZero()
        {
            var controller = new InventoryItemsController(repo.Object);

            var result = await controller.Adjust(7, new AdjustStockRequest { Delta = -11 });

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(10, item.QuantityOnHand);
            repo.Verify(r => r.SaveAsync(), Times.Never);
        }

        [TestMethod]
        public async Task Adjust_ZeroDelta_IsBadRequest()
        {
            var controller = new InventoryItemsController(repo.Object);

            var result = await controller.Adjust(7, new AdjustStockRequest { Delta = 0 });

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task Adjust_UnknownItem_IsNotFound()
        {
            var controller = new InventoryItemsController(repo.Object);

            var result = await controller.Adjust(99, new AdjustStockRequest { Delta = 1 });

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
