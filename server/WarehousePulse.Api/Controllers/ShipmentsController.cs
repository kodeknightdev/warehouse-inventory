using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;
using WarehousePulse.Api.Models.Dto;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : ApiController
    {
        private readonly WarehouseContext db;

        public ShipmentsController(WarehouseContext db)
        {
            this.db = db;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(string status = null)
        {
            var query = db.Shipments.AsQueryable();

            ShipmentStatus parsed;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, true, out parsed))
                query = query.Where(s => s.Status == parsed);

            var shipments = await query
                .OrderByDescending(s => s.CreatedAt)
                .Take(200)
                .ToListAsync();

            return Ok(shipments.Select(s => ShipmentDto.From(s, includeLines: false)));
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
                OccurredAt = DateTime.Now
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
                foreach (var line in shipment.Lines)
                {
                    var item = line.InventoryItem;
                    var newQty = item.QuantityOnHand + sign * line.Quantity;
                    if (newQty < 0)
                        return BadRequest("Not enough stock for " + item.Sku);

                    item.QuantityOnHand = newQty;
                    item.UpdatedAt = DateTime.UtcNow;
                    db.StockMovements.Add(new StockMovement
                    {
                        InventoryItemId = item.Id,
                        Delta = sign * line.Quantity,
                        QuantityAfter = newQty,
                        Reason = "Shipment " + shipment.Reference,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            shipment.Status = request.Status;
            if (request.Status == ShipmentStatus.InTransit && shipment.ShippedAt == null)
                shipment.ShippedAt = DateTime.Now;
            if (request.Status == ShipmentStatus.Delivered)
                shipment.DeliveredAt = DateTime.Now;

            shipment.Events.Add(new TrackingEvent
            {
                Status = request.Status,
                Location = request.Location,
                Note = request.Note,
                OccurredAt = DateTime.Now
            });

            await db.SaveChangesAsync();
            return Ok(ShipmentDto.From(shipment));
        }
    }
}
