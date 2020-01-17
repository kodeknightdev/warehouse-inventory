using System;
using System.Linq;
using System.Net;
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

            var sku = request.Sku.Trim().ToUpperInvariant();
            if (await repo.SkuExistsAsync(sku))
                return Content(HttpStatusCode.Conflict, new { message = "SKU " + sku + " already exists" });

            var item = new InventoryItem
            {
                Sku = sku,
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

            var sku = item.Sku.Trim().ToUpperInvariant();
            if (await repo.SkuExistsAsync(sku, id))
                return Content(HttpStatusCode.Conflict, new { message = "SKU " + sku + " already exists" });

            existing.Sku = sku;
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

        [HttpPost, Route("{id:int}/adjust")]
        public async Task<IHttpActionResult> Adjust(int id, AdjustStockRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            if (request.Delta == 0)
                return BadRequest("Delta can't be zero");

            var item = await repo.GetAsync(id);
            if (item == null)
                return NotFound();

            var newQty = item.QuantityOnHand + request.Delta;
            if (newQty < 0)
                return BadRequest("Not enough stock, only " + item.QuantityOnHand + " on hand");

            item.QuantityOnHand = newQty;
            item.UpdatedAt = DateTime.UtcNow;
            repo.AddMovement(new StockMovement
            {
                InventoryItemId = item.Id,
                Delta = request.Delta,
                QuantityAfter = newQty,
                Reason = request.Reason,
                CreatedAt = item.UpdatedAt
            });
            await repo.SaveAsync();

            return Ok(InventoryItemDto.From(item));
        }

        [HttpGet, Route("{id:int}/movements")]
        public async Task<IHttpActionResult> Movements(int id, int take = 50)
        {
            var movements = await repo.GetMovementsAsync(id, Math.Min(take, 200));
            return Ok(movements.Select(m => new
            {
                m.Id,
                m.Delta,
                m.QuantityAfter,
                m.Reason,
                m.CreatedAt
            }));
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
