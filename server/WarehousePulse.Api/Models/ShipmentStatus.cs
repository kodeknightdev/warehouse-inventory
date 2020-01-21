namespace WarehousePulse.Api.Models
{
    public enum ShipmentStatus
    {
        Pending = 0,
        Picking = 1,
        Packed = 2,
        InTransit = 3,
        Delivered = 4,
        Cancelled = 9
    }

    public enum ShipmentDirection
    {
        Outbound = 0,
        Inbound = 1
    }
}
