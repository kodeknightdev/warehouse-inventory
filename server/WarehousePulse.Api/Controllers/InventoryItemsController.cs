using System;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/items")]
    public class InventoryItemsController : ApiController
    {
        private readonly IInventoryRepository repo;

        public InventoryItemsController(IInventoryRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            var items = await repo.GetAllAsync();
            return Ok(items);
        }

        [HttpGet, Route("{id:int}", Name = "GetItem")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var item = await repo.GetAsync(id);
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
            repo.Add(item);
            await repo.SaveAsync();

            return CreatedAtRoute("GetItem", new { id = item.Id }, item);
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, InventoryItem item)
        {
            var existing = await repo.GetAsync(id);
            if (existing == null)
                return NotFound();

            existing.Sku = item.Sku;
            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.Category = item.Category;
            existing.UnitOfMeasure = item.UnitOfMeasure;
            existing.UnitCost = item.UnitCost;
            existing.QuantityOnHand = item.QuantityOnHand;
            existing.LocationId = item.LocationId;
            existing.UpdatedAt = DateTime.UtcNow;

            await repo.SaveAsync();
            return Ok(existing);
        }

        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var item = await repo.GetAsync(id);
            if (item == null)
                return NotFound();

            repo.Remove(item);
            await repo.SaveAsync();
            return Ok();
        }
    }
}
