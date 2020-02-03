using System.Web.Http;
using Swashbuckle.Application;
using WarehousePulse.Api;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WarehousePulse.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            // trimmed down from the generated file, see /swagger
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Warehouse Pulse API");
                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi();
        }
    }
}
