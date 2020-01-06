using System;
using System.Web.Http;

namespace WarehousePulse.Api.Controllers
{
    [RoutePrefix("api/ping")]
    public class PingController : ApiController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new { status = "ok", serverTime = DateTime.UtcNow });
        }
    }
}
