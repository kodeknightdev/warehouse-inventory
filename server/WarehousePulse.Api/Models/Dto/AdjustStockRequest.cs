using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models.Dto
{
    public class AdjustStockRequest
    {
        // positive = received, negative = picked/shipped/written off
        public int Delta { get; set; }

        [StringLength(200)]
        public string Reason { get; set; }
    }
}
