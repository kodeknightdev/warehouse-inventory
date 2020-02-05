using System.Web;
using System.Web.Http;
using WarehousePulse.Api.Background;

namespace WarehousePulse.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutofacConfig.Register(GlobalConfiguration.Configuration);

            LowStockMonitor.Start();
        }
    }
}
