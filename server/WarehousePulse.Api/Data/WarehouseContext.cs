using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WarehousePulse.Api.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext() : base("name=WarehouseContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // singular table names, I don't want "InventoryItems" style tables
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
