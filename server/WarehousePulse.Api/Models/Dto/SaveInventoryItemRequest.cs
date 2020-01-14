using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models.Dto
{
    public class SaveInventoryItemRequest
    {
        [Required, StringLength(32)]
        public string Sku { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        // only used on create, after that quantity changes go through adjustments
        public int QuantityOnHand { get; set; }

        [Required, StringLength(16)]
        public string UnitOfMeasure { get; set; }

        [Range(0, 1000000)]
        public decimal UnitCost { get; set; }

        public int? LocationId { get; set; }
    }
}
