using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
