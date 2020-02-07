using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/alerts")]
    public class AlertsController : ApiController
    {
        private readonly WarehouseContext db;

        public AlertsController(WarehouseContext db)
        {
            this.db = db;
        }

        // open alerts = not resolved and nobody has acknowledged them yet
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetOpen(bool includeAcknowledged = false)
        {
            var query = db.LowStockAlerts.Where(a => a.ResolvedAt == null);
            if (!includeAcknowledged)
                query = query.Where(a => a.AcknowledgedAt == null);

            var alerts = await query
                .OrderBy(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.InventoryItemId,
                    a.InventoryItem.Sku,
                    a.InventoryItem.Name,
                    CurrentQuantity = a.InventoryItem.QuantityOnHand,
                    a.QuantityAtAlert,
                    a.ReorderLevel,
                    a.CreatedAt,
                    a.AcknowledgedAt
                })
                .ToListAsync();

            return Ok(alerts);
        }

        [HttpPost, Route("{id:int}/acknowledge")]
        public async Task<IHttpActionResult> Acknowledge(int id)
        {
            var alert = await db.LowStockAlerts.FindAsync(id);
            if (alert == null)
                return NotFound();

            if (alert.AcknowledgedAt == null)
            {
                alert.AcknowledgedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
