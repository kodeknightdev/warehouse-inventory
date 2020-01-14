using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;
using WarehousePulse.Api.Models.Dto;

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
            return Ok(items.Select(InventoryItemDto.From));
        }

        [HttpGet, Route("{id:int}", Name = "GetItem")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var item = await repo.GetAsync(id);
            if (item == null)
                return NotFound();

            return Ok(InventoryItemDto.From(item));
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(SaveInventoryItemRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var item = new InventoryItem
            {
                Sku = request.Sku.Trim().ToUpperInvariant(),
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                QuantityOnHand = request.QuantityOnHand,
                UnitOfMeasure = request.UnitOfMeasure,
                UnitCost = request.UnitCost,
                LocationId = request.LocationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            repo.Add(item);
            await repo.SaveAsync();

            return CreatedAtRoute("GetItem", new { id = item.Id }, InventoryItemDto.From(item));
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, SaveInventoryItemRequest item)
        {
            if (item == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await repo.GetAsync(id);
            if (existing == null)
                return NotFound();

            existing.Sku = item.Sku.Trim().ToUpperInvariant();
            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.Category = item.Category;
            existing.UnitOfMeasure = item.UnitOfMeasure;
            existing.UnitCost = item.UnitCost;
            existing.LocationId = item.LocationId;
            existing.UpdatedAt = DateTime.UtcNow;

            await repo.SaveAsync();
            return Ok(InventoryItemDto.From(existing));
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
