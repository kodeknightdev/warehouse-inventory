using System;

namespace WarehousePulse.Api.Models
{
    public class LowStockAlert
    {
        public int Id { get; set; }

        public int InventoryItemId { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }

        public int QuantityAtAlert { get; set; }

        public int ReorderLevel { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? AcknowledgedAt { get; set; }

        // set once stock goes back above the reorder level
        public DateTime? ResolvedAt { get; set; }
    }
}
