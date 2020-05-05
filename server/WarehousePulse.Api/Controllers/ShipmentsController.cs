using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;
using WarehousePulse.Api.Models.Dto;
using WarehousePulse.Api.Services;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : ApiController
    {
        private readonly WarehouseContext db;
        private readonly StockService stock;

        public ShipmentsController(WarehouseContext db, StockService stock)
        {
            this.db = db;
            this.stock = stock;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(string status = null, string carrier = null, string search = null)
        {
            var query = db.Shipments.AsQueryable();

            ShipmentStatus parsed;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, true, out parsed))
                query = query.Where(s => s.Status == parsed);

            if (!string.IsNullOrWhiteSpace(carrier))
                query = query.Where(s => s.Carrier == carrier);

            // reference or tracking number, people paste either one
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(s => s.Reference.Contains(term) || s.TrackingNumber.Contains(term));
            }

            // need the lines for TotalUnits, without this the list always showed 0
            var shipments = await query
                .Include(s => s.Lines)
                .OrderByDescending(s => s.CreatedAt)
                .Take(200)
                .ToListAsync();

            return Ok(shipments.Select(s => ShipmentDto.From(s, includeLines: false)));
        }

        [HttpGet, Route("carriers")]
        public async Task<IHttpActionResult> Carriers()
        {
            var carriers = await db.Shipments
                .Where(s => s.Carrier != null && s.Carrier != "")
                .Select(s => s.Carrier)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            return Ok(carriers);
        }

        [HttpGet, Route("{id:int}", Name = "GetShipment")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var shipment = await db.Shipments
                .Include(s => s.Lines.Select(l => l.InventoryItem))
                .Include(s => s.Events)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
                return NotFound();

            return Ok(ShipmentDto.From(shipment));
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(CreateShipmentRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var itemIds = request.Lines.Select(l => l.InventoryItemId).Distinct().ToList();
            var found = await db.InventoryItems.CountAsync(i => itemIds.Contains(i.Id));
            if (found != itemIds.Count)
                return BadRequest("One or more items don't exist");

            var shipment = new Shipment
            {
                Reference = "SHP-" + DateTime.UtcNow.ToString("yyMMddHHmmss"),
                Direction = request.Direction,
                Status = ShipmentStatus.Pending,
                Carrier = request.Carrier,
                TrackingNumber = request.TrackingNumber,
                Destination = request.Destination,
                ExpectedAt = request.ExpectedAt,
                CreatedAt = DateTime.UtcNow
            };

            shipment.Events.Add(new TrackingEvent
            {
                Status = ShipmentStatus.Pending,
                Note = "Shipment created",
                OccurredAt = DateTime.UtcNow
            });

            foreach (var line in request.Lines)
                shipment.Lines.Add(new ShipmentLine { InventoryItemId = line.InventoryItemId, Quantity = line.Quantity });

            db.Shipments.Add(shipment);
            await db.SaveChangesAsync();

            return CreatedAtRoute("GetShipment", new { id = shipment.Id }, ShipmentDto.From(shipment));
        }

        [HttpPost, Route("{id:int}/status")]
        public async Task<IHttpActionResult> UpdateStatus(int id, UpdateShipmentStatusRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var shipment = await db.Shipments
                .Include(s => s.Lines.Select(l => l.InventoryItem))
                .Include(s => s.Events)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
                return NotFound();

            if (!ShipmentStatusRules.CanMove(shipment.Status, request.Status))
                return BadRequest(string.Format("Can't change status from {0} to {1}", shipment.Status, request.Status));

            // outbound stock leaves when the truck leaves, inbound stock counts once it's delivered
            var outboundDispatch = shipment.Direction == ShipmentDirection.Outbound
                && request.Status == ShipmentStatus.InTransit && shipment.ShippedAt == null;
            var inboundReceipt = shipment.Direction == ShipmentDirection.Inbound
                && request.Status == ShipmentStatus.Delivered && shipment.DeliveredAt == null;

            if (outboundDispatch || inboundReceipt)
            {
                var sign = outboundDispatch ? -1 : 1;
                try
                {
                    foreach (var line in shipment.Lines)
                        db.StockMovements.Add(stock.Apply(line.InventoryItem, sign * line.Quantity, "Shipment " + shipment.Reference, DateTime.UtcNow));
                }
                catch (InsufficientStockException ex)
                {
                    // nothing has been saved yet, the context is thrown away with the request
                    return BadRequest(ex.Message);
                }
            }

            shipment.Status = request.Status;
            if (request.Status == ShipmentStatus.InTransit && shipment.ShippedAt == null)
                shipment.ShippedAt = DateTime.UtcNow;
            if (request.Status == ShipmentStatus.Delivered)
                shipment.DeliveredAt = DateTime.UtcNow;

            shipment.Events.Add(new TrackingEvent
            {
                Status = request.Status,
                Location = request.Location,
                Note = request.Note,
                OccurredAt = DateTime.UtcNow
            });

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Content(HttpStatusCode.Conflict, new { message = "Stock for one of the items changed while updating, try again." });
            }
            return Ok(ShipmentDto.From(shipment));
        }
    }
}
