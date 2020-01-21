using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models
{
    public class Shipment
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Reference { get; set; }

        public ShipmentDirection Direction { get; set; }

        public ShipmentStatus Status { get; set; }

        [StringLength(50)]
        public string Carrier { get; set; }

        [StringLength(64)]
        public string TrackingNumber { get; set; }

        // customer / supplier name + address, kept as free text for now
        [StringLength(500)]
        public string Destination { get; set; }

        public DateTime? ExpectedAt { get; set; }

        public DateTime? ShippedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ShipmentLine> Lines { get; set; } = new List<ShipmentLine>();
    }
}
