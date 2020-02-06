using System;
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
            timer = new Timer(OnTick, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5));
        }

        public static void Start()
        {
            if (instance != null)
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
