using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousePulse.Api.Models
{
    // A bin/shelf location on the floor, e.g. "A-03-2" (aisle-rack-shelf)
    public class Location
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Zone { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public virtual ICollection<InventoryItem> Items { get; set; }
    }
}
