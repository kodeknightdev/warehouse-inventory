using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web.Hosting;
using WarehousePulse.Api.Data;
using WarehousePulse.Api.Services;

namespace WarehousePulse.Api.Background
{
    /// <summary>
    /// Checks stock levels on a timer while the app pool is up.
    /// Registered with HostingEnvironment so IIS gives us a chance to stop cleanly on recycle.
    /// </summary>
    public class LowStockMonitor : IRegisteredObject
    {
        private static LowStockMonitor instance;

        private readonly object sync = new object();
        private readonly Timer timer;
        private bool running;
        private bool stopping;

        private LowStockMonitor()
        {
            timer = new Timer(OnTick, null, TimeSpan.FromSeconds(30), CheckInterval());
        }

        private static TimeSpan CheckInterval()
        {
            int minutes;
            var setting = ConfigurationManager.AppSettings["LowStockCheckMinutes"];
            if (!int.TryParse(setting, out minutes) || minutes < 1)
                minutes = 5;
            return TimeSpan.FromMinutes(minutes);
        }

        public static void Start()
        {
            if (instance != null)
                return;

            var enabled = ConfigurationManager.AppSettings["LowStockMonitorEnabled"];
            if (string.Equals(enabled, "false", StringComparison.OrdinalIgnoreCase))
                return;

            instance = new LowStockMonitor();
            HostingEnvironment.RegisterObject(instance);
        }

        private void OnTick(object state)
        {
            lock (sync)
            {
                // skip if the previous run is still going
                if (running || stopping)
                    return;
                running = true;
            }

            try
            {
                CheckStock();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Low stock check failed: " + ex);
            }
            finally
            {
                lock (sync)
                {
                    running = false;
                }
            }
        }

        private void CheckStock()
        {
            using (var db = new WarehouseContext())
            {
                var raised = new LowStockChecker(db).Run(DateTime.UtcNow);
                if (raised > 0)
                    Trace.TraceWarning("Low stock check raised {0} alert(s)", raised);
            }
        }

        public void Stop(bool immediate)
        {
            lock (sync)
            {
                stopping = true;
            }

            timer.Dispose();
            HostingEnvironment.UnregisterObject(this);
        }
    }
}
