namespace WarehousePulse.Api.Models
{
    public class ShipmentLine
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        public virtual Shipment Shipment { get; set; }

        public int InventoryItemId { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }

        public int Quantity { get; set; }
    }
}
