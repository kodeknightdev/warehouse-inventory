using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;
using WarehousePulse.Api.Models.Dto;
using WarehousePulse.Api.Services;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/items")]
    public class InventoryItemsController : ApiController
    {
        private readonly IInventoryRepository repo;
        private readonly StockService stock;

        public InventoryItemsController(IInventoryRepository repo, StockService stock)
        {
            this.repo = repo;
            this.stock = stock;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(string search = null, string category = null, int? locationId = null, int page = 1, int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 25;

            var result = await repo.SearchAsync(search, category, locationId, page, pageSize);
            return Ok(new PagedResult<InventoryItemDto>
            {
                Items = result.Item1.Select(InventoryItemDto.From),
                Total = result.Item2,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet, Route("categories")]
        public async Task<IHttpActionResult> Categories()
        {
            return Ok(await repo.GetCategoriesAsync());
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
                ReorderLevel = request.ReorderLevel,
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
            existing.ReorderLevel = item.ReorderLevel;
            existing.LocationId = item.LocationId;
            existing.UpdatedAt = DateTime.UtcNow;

            return await SaveOrConflict(() => Ok(InventoryItemDto.From(existing)));
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

            try
            {
                repo.AddMovement(stock.Apply(item, request.Delta, request.Reason, DateTime.UtcNow));
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }

            return await SaveOrConflict(() => Ok(InventoryItemDto.From(item)));
        }

        private async Task<IHttpActionResult> SaveOrConflict(Func<IHttpActionResult> onSuccess)
        {
            try
            {
                await repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Content(HttpStatusCode.Conflict, new { message = "The item was changed by someone else, reload and try again." });
            }
            return onSuccess();
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
