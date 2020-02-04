using System;

namespace WarehousePulse.Api.Models.Dto
{
    public class InventoryItemDto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsLowStock { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitCost { get; set; }
        public int? LocationId { get; set; }
        public string LocationCode { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static InventoryItemDto From(InventoryItem item)
        {
            return new InventoryItemDto
            {
                Id = item.Id,
                Sku = item.Sku,
                Name = item.Name,
                Description = item.Description,
                Category = item.Category,
                QuantityOnHand = item.QuantityOnHand,
                ReorderLevel = item.ReorderLevel,
                IsLowStock = item.ReorderLevel > 0 && item.QuantityOnHand <= item.ReorderLevel,
                UnitOfMeasure = item.UnitOfMeasure,
                UnitCost = item.UnitCost,
                LocationId = item.LocationId,
                LocationCode = item.Location != null ? item.Location.Code : null,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
