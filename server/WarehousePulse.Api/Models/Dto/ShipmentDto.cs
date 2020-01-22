using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehousePulse.Api.Models.Dto
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public string Destination { get; set; }
        public DateTime? ExpectedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalUnits { get; set; }
        public List<ShipmentLineDto> Lines { get; set; }

        public static ShipmentDto From(Shipment s, bool includeLines = true)
        {
            var dto = new ShipmentDto
            {
                Id = s.Id,
                Reference = s.Reference,
                Direction = s.Direction.ToString(),
                Status = s.Status.ToString(),
                Carrier = s.Carrier,
                TrackingNumber = s.TrackingNumber,
                Destination = s.Destination,
                ExpectedAt = s.ExpectedAt,
                ShippedAt = s.ShippedAt,
                DeliveredAt = s.DeliveredAt,
                CreatedAt = s.CreatedAt,
                TotalUnits = s.Lines.Sum(l => l.Quantity)
            };

            if (includeLines)
            {
                dto.Lines = s.Lines.Select(l => new ShipmentLineDto
                {
                    Id = l.Id,
                    InventoryItemId = l.InventoryItemId,
                    Sku = l.InventoryItem != null ? l.InventoryItem.Sku : null,
                    Name = l.InventoryItem != null ? l.InventoryItem.Name : null,
                    Quantity = l.Quantity
                }).ToList();
            }

            return dto;
        }
    }

    public class ShipmentLineDto
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
