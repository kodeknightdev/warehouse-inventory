using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePulse.Api.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        [Required, StringLength(32)]
        [Index("IX_InventoryItem_Sku", IsUnique = true)]
        public string Sku { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        public int QuantityOnHand { get; set; }

        // "ea", "box", "pallet" etc.
        [Required, StringLength(16)]
        public string UnitOfMeasure { get; set; }

        [Column(TypeName = "decimal")]
        public decimal UnitCost { get; set; }

        public int? LocationId { get; set; }

        public virtual Location Location { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
