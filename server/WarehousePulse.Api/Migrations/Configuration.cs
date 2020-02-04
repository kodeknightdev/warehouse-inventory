namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WarehousePulse.Api.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WarehousePulse.Api.Data.WarehouseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WarehousePulse.Api.Data.WarehouseContext context)
        {
            // dev data only - AddOrUpdate keys on Code/Sku so re-running update-database is safe
            context.Locations.AddOrUpdate(l => l.Code,
                new Location { Code = "A-01-1", Zone = "Bulk", Description = "Aisle A, rack 1, floor" },
                new Location { Code = "A-01-2", Zone = "Bulk", Description = "Aisle A, rack 1, shelf 2" },
                new Location { Code = "B-04-1", Zone = "Small parts", Description = "Aisle B, rack 4" },
                new Location { Code = "B-04-3", Zone = "Small parts", Description = "Aisle B, rack 4, top" },
                new Location { Code = "C-02-1", Zone = "Packaging", Description = "Aisle C, rack 2" },
                new Location { Code = "DOCK-1", Zone = "Receiving", Description = "Inbound staging" });
            context.SaveChanges();

            var loc = context.Locations.ToDictionary(l => l.Code, l => l.Id);
            var now = DateTime.UtcNow;

            context.InventoryItems.AddOrUpdate(i => i.Sku,
                Item("PAL-STD-1200", "Standard pallet 1200x800", "Packaging", 140, 50, "ea", 9.50m, loc["A-01-1"], now),
                Item("BOX-S-01", "Shipping box small 30x20x15", "Packaging", 820, 200, "ea", 0.42m, loc["C-02-1"], now),
                Item("BOX-M-01", "Shipping box medium 40x30x25", "Packaging", 615, 200, "ea", 0.61m, loc["C-02-1"], now),
                Item("BOX-L-01", "Shipping box large 60x40x40", "Packaging", 38, 50, "ea", 0.95m, loc["C-02-1"], now),
                Item("TAPE-48-CLR", "Packing tape 48mm clear", "Packaging", 96, 48, "roll", 1.80m, loc["C-02-1"], now),
                Item("WRAP-500", "Stretch wrap 500mm", "Packaging", 12, 10, "roll", 14.25m, loc["A-01-2"], now),
                Item("BLT-M8-40", "Hex bolt M8x40 zinc", "Fasteners", 4200, 1000, "ea", 0.07m, loc["B-04-1"], now),
                Item("NUT-M8", "Hex nut M8 zinc", "Fasteners", 3900, 1000, "ea", 0.03m, loc["B-04-1"], now),
                Item("WSH-M8", "Flat washer M8", "Fasteners", 350, 500, "ea", 0.02m, loc["B-04-1"], now),
                Item("GLV-NIT-L", "Nitrile gloves L (box of 100)", "Safety", 24, 10, "box", 7.90m, loc["B-04-3"], now),
                Item("VEST-HV-XL", "Hi-vis vest XL", "Safety", 6, 5, "ea", 4.20m, loc["B-04-3"], now),
                Item("LBL-4X6", "Thermal labels 4x6 (roll of 250)", "Labels", 40, 20, "roll", 6.75m, loc["B-04-3"], now));
            context.SaveChanges();
        }

        private static InventoryItem Item(string sku, string name, string category, int qty, int reorderLevel, string uom, decimal cost, int locationId, DateTime now)
        {
            return new InventoryItem
            {
                Sku = sku,
                Name = name,
                Category = category,
                QuantityOnHand = qty,
                ReorderLevel = reorderLevel,
                UnitOfMeasure = uom,
                UnitCost = cost,
                LocationId = locationId,
                CreatedAt = now,
                UpdatedAt = now
            };
        }
    }
}
