using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext() : base("name=WarehouseContext")
        {
            // no lazy loading, use Include() explicitly
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // singular table names, I don't want "InventoryItems" style tables
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.UnitCost)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
