using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WarehousePulse.Api.Infrastructure;

namespace WarehousePulse.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // JSON only, camelCase so the front end doesn't have to deal with PascalCase
            var xml = config.Formatters.XmlFormatter;
            config.Formatters.Remove(xml);

            var json = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // item -> location -> items -> ... blows up the serializer
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            // everything is stored as UTC but EF hands back Kind=Unspecified, so the "Z" was missing
            // and the browser treated the times as local
            json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            var origins = ConfigurationManager.AppSettings["CorsOrigins"];
            if (!string.IsNullOrWhiteSpace(origins))
                config.EnableCors(new EnableCorsAttribute(origins, "*", "*"));

            config.Filters.Add(new UnhandledExceptionFilter());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
