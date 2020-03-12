using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Tests.Models
{
    [TestClass]
    public class ShipmentStatusRulesTests
    {
        [TestMethod]
        public void CanMoveForward()
        {
            Assert.IsTrue(ShipmentStatusRules.CanMove(ShipmentStatus.Pending, ShipmentStatus.Picking));
            Assert.IsTrue(ShipmentStatusRules.CanMove(ShipmentStatus.Packed, ShipmentStatus.InTransit));
            Assert.IsTrue(ShipmentStatusRules.CanMove(ShipmentStatus.InTransit, ShipmentStatus.Delivered));
        }

        [TestMethod]
        public void CantGoBackwards()
        {
            Assert.IsFalse(ShipmentStatusRules.CanMove(ShipmentStatus.Delivered, ShipmentStatus.InTransit));
            Assert.IsFalse(ShipmentStatusRules.CanMove(ShipmentStatus.InTransit, ShipmentStatus.Packed));
        }

        [TestMethod]
        public void CantCancelOnceShipped()
        {
            Assert.IsTrue(ShipmentStatusRules.CanMove(ShipmentStatus.Packed, ShipmentStatus.Cancelled));
            Assert.IsFalse(ShipmentStatusRules.CanMove(ShipmentStatus.InTransit, ShipmentStatus.Cancelled));
            Assert.IsFalse(ShipmentStatusRules.CanMove(ShipmentStatus.Cancelled, ShipmentStatus.Pending));
        }

        [TestMethod]
        public void SameStatus_OnlyAllowedWhileInTransit()
        {
            Assert.IsTrue(ShipmentStatusRules.CanMove(ShipmentStatus.InTransit, ShipmentStatus.InTransit));
            Assert.IsFalse(ShipmentStatusRules.CanMove(ShipmentStatus.Delivered, ShipmentStatus.Delivered));
        }
    }
}
