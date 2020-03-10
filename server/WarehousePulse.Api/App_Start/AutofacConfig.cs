using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Services;

namespace WarehousePulse.Api
{
    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // one context per request, disposed by autofac at the end of the request
            builder.RegisterType<WarehouseContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<StockService>().AsSelf().SingleInstance();
            builder.RegisterType<InventoryRepository>().As<IInventoryRepository>().InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
