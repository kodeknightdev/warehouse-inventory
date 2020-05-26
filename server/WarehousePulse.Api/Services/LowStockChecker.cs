using System;
using System.Linq;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Services
{
    public class LowStockCheckResult
    {
        public int Raised { get; set; }
        public int Resolved { get; set; }
    }

    public class LowStockChecker
    {
        private readonly WarehouseContext db;

        public LowStockChecker(WarehouseContext db)
        {
            this.db = db;
        }

        public LowStockCheckResult Run(DateTime now)
        {
            // items that already have an open alert are skipped, otherwise we'd add
            // a new alert every 5 minutes for the same item
            var low = db.InventoryItems
                .Where(i => i.ReorderLevel > 0 && i.QuantityOnHand <= i.ReorderLevel)
                .Where(i => !db.LowStockAlerts.Any(a => a.InventoryItemId == i.Id && a.ResolvedAt == null))
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

            // close alerts once stock is back above the reorder level (or alerts were switched off
            // for the item). Without this an item could never alert again after the first time.
            var recovered = db.LowStockAlerts
                .Where(a => a.ResolvedAt == null)
                .Where(a => a.InventoryItem.ReorderLevel == 0 || a.InventoryItem.QuantityOnHand > a.InventoryItem.ReorderLevel)
                .ToList();

            foreach (var alert in recovered)
                alert.ResolvedAt = now;

            if (low.Count > 0 || recovered.Count > 0)
                db.SaveChanges();

            return new LowStockCheckResult { Raised = low.Count, Resolved = recovered.Count };
        }
    }
}
