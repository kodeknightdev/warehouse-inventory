using System.Collections.Generic;

namespace WarehousePulse.Api.Models
{
    public static class ShipmentStatusRules
    {
        // forward only. Cancelled is allowed until the shipment has left the building
        private static readonly Dictionary<ShipmentStatus, ShipmentStatus[]> allowed = new Dictionary<ShipmentStatus, ShipmentStatus[]>
        {
            { ShipmentStatus.Pending, new[] { ShipmentStatus.Picking, ShipmentStatus.Packed, ShipmentStatus.InTransit, ShipmentStatus.Cancelled } },
            { ShipmentStatus.Picking, new[] { ShipmentStatus.Packed, ShipmentStatus.InTransit, ShipmentStatus.Cancelled } },
            { ShipmentStatus.Packed, new[] { ShipmentStatus.InTransit, ShipmentStatus.Cancelled } },
            { ShipmentStatus.InTransit, new[] { ShipmentStatus.Delivered } },
            { ShipmentStatus.Delivered, new ShipmentStatus[0] },
            { ShipmentStatus.Cancelled, new ShipmentStatus[0] }
        };

        public static bool CanMove(ShipmentStatus from, ShipmentStatus to)
        {
            // re-posting the same status is fine, it just adds a tracking event (e.g. new scan location)
            if (from == to)
                return from == ShipmentStatus.InTransit;

            return System.Array.IndexOf(allowed[from], to) >= 0;
        }
    }
}
