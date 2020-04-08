using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/locations")]
    public class LocationsController : ApiController
    {
        private readonly WarehouseContext db;

        public LocationsController(WarehouseContext db)
        {
            this.db = db;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            var locations = await db.Locations
                .OrderBy(l => l.Code)
                .Select(l => new { l.Id, l.Code, l.Zone, l.Description, ItemCount = l.Items.Count() })
                .ToListAsync();
            return Ok(locations);
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(Location location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.Locations.Add(location);
            await db.SaveChangesAsync();
            return Ok(location);
        }
    }
}
