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
                UnitOfMeasure = item.UnitOfMeasure,
                UnitCost = item.UnitCost,
                LocationId = item.LocationId,
                LocationCode = item.Location != null ? item.Location.Code : null,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
