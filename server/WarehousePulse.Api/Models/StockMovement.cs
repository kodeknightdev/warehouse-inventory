using System;
using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models
{
    // audit trail for every quantity change on an item
    public class StockMovement
    {
        public long Id { get; set; }

        public int InventoryItemId { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }

        public int Delta { get; set; }

        public int QuantityAfter { get; set; }

        [StringLength(200)]
        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
