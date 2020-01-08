using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/items")]
    public class InventoryItemsController : ApiController
    {
        private readonly WarehouseContext db = new WarehouseContext();

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            var items = await db.InventoryItems.OrderBy(i => i.Sku).ToListAsync();
            return Ok(items);
        }

        [HttpGet, Route("{id:int}", Name = "GetItem")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var item = await db.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(InventoryItem item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            item.CreatedAt = item.UpdatedAt = DateTime.UtcNow;
            db.InventoryItems.Add(item);
            await db.SaveChangesAsync();

            return CreatedAtRoute("GetItem", new { id = item.Id }, item);
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, InventoryItem item)
        {
            var existing = await db.InventoryItems.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Sku = item.Sku;
            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.Category = item.Category;
            existing.UnitOfMeasure = item.UnitOfMeasure;
            existing.UnitCost = item.UnitCost;
            existing.QuantityOnHand = item.QuantityOnHand;
            existing.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var item = await db.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            db.InventoryItems.Remove(item);
            await db.SaveChangesAsync();
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
