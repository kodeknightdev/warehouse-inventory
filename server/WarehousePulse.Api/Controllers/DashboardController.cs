using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly WarehouseContext db;

        public DashboardController(WarehouseContext db)
        {
            this.db = db;
        }

        [HttpGet, Route("summary")]
        public async Task<IHttpActionResult> Summary()
        {
            var items = db.InventoryItems;

            var totalSkus = await items.CountAsync();
            var totalUnits = await items.SumAsync(i => (int?)i.QuantityOnHand) ?? 0;
            var stockValue = await items.SumAsync(i => (decimal?)(i.QuantityOnHand * i.UnitCost)) ?? 0m;
            var lowStock = await items.CountAsync(i => i.ReorderLevel > 0 && i.QuantityOnHand <= i.ReorderLevel);
            var openAlerts = await db.LowStockAlerts.CountAsync(a => a.ResolvedAt == null && a.AcknowledgedAt == null);

            var byStatus = await db.Shipments
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var byCategory = await items
                .GroupBy(i => i.Category ?? "Uncategorized")
                .Select(g => new
                {
                    Category = g.Key,
                    Units = g.Sum(i => i.QuantityOnHand),
                    Value = g.Sum(i => i.QuantityOnHand * i.UnitCost)
                })
                .OrderByDescending(x => x.Value)
                .ToListAsync();

            Func<ShipmentStatus, int> count = st => byStatus.Where(x => x.Status == st).Select(x => x.Count).FirstOrDefault();

            return Ok(new
            {
                totalSkus,
                totalUnits,
                stockValue,
                lowStockItems = lowStock,
                openAlerts,
                shipments = new
                {
                    pending = count(ShipmentStatus.Pending) + count(ShipmentStatus.Picking) + count(ShipmentStatus.Packed),
                    inTransit = count(ShipmentStatus.InTransit),
                    delivered = count(ShipmentStatus.Delivered)
                },
                stockByCategory = byCategory
            });
        }
    }
}
