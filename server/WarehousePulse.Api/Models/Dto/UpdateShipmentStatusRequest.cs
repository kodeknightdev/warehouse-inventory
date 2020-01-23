using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models.Dto
{
    public class UpdateShipmentStatusRequest
    {
        public ShipmentStatus Status { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(300)]
        public string Note { get; set; }
    }
}
