using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models.Dto
{
    public class CreateShipmentRequest
    {
        public ShipmentDirection Direction { get; set; }

        [StringLength(50)]
        public string Carrier { get; set; }

        [StringLength(64)]
        public string TrackingNumber { get; set; }

        [Required, StringLength(500)]
        public string Destination { get; set; }

        public DateTime? ExpectedAt { get; set; }

        [Required, MinLength(1)]
        public List<CreateShipmentLine> Lines { get; set; }
    }

    public class CreateShipmentLine
    {
        [Range(1, int.MaxValue, ErrorMessage = "Item is required")]
        public int InventoryItemId { get; set; }

        [Range(1, 1000000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
