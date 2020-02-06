using System;
using System.Linq;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Services
{
    public class LowStockChecker
    {
        private readonly WarehouseContext db;

        public LowStockChecker(WarehouseContext db)
        {
            this.db = db;
        }

        /// <returns>number of new alerts raised</returns>
        public int Run(DateTime now)
        {
            var low = db.InventoryItems
                .Where(i => i.ReorderLevel > 0 && i.QuantityOnHand <= i.ReorderLevel)
                .ToList();

            foreach (var item in low)
            {
                db.LowStockAlerts.Add(new LowStockAlert
                {
                    InventoryItemId = item.Id,
                    QuantityAtAlert = item.QuantityOnHand,
                    ReorderLevel = item.ReorderLevel,
                    CreatedAt = now
                });
            }

            db.SaveChanges();
            return low.Count;
        }
    }
}
