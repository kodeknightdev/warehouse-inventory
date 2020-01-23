using System;
using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models
{
    public class TrackingEvent
    {
        public long Id { get; set; }

        public int ShipmentId { get; set; }

        public virtual Shipment Shipment { get; set; }

        public ShipmentStatus Status { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(300)]
        public string Note { get; set; }

        public DateTime OccurredAt { get; set; }
    }
}
