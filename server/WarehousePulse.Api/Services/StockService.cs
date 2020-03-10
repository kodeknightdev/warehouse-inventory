using System;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Services
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(InventoryItem item, int requested)
            : base(string.Format("Not enough stock for {0}, {1} on hand, {2} requested", item.Sku, item.QuantityOnHand, requested))
        {
        }
    }

    /// <summary>
    /// The one place that changes QuantityOnHand. Callers add the returned movement to the context and save.
    /// </summary>
    public class StockService
    {
        public StockMovement Apply(InventoryItem item, int delta, string reason, DateTime now)
        {
            if (delta == 0)
                throw new ArgumentException("Delta can't be zero", "delta");

            var newQty = item.QuantityOnHand + delta;
            if (newQty < 0)
                throw new InsufficientStockException(item, -delta);

            item.QuantityOnHand = newQty;
            item.UpdatedAt = now;

            return new StockMovement
            {
                InventoryItemId = item.Id,
                Delta = delta,
                QuantityAfter = newQty,
                Reason = reason,
                CreatedAt = now
            };
        }
    }
}
