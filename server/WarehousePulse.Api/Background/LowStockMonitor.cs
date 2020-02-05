using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using WarehousePulse.Api.Data;

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
                var low = db.InventoryItems
                    .Where(i => i.ReorderLevel > 0 && i.QuantityOnHand <= i.ReorderLevel)
                    .Select(i => new { i.Sku, i.QuantityOnHand, i.ReorderLevel })
                    .ToList();

                foreach (var item in low)
                    Trace.TraceWarning("Low stock: {0} has {1} (reorder at {2})", item.Sku, item.QuantityOnHand, item.ReorderLevel);
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
