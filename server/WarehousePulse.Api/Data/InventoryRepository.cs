using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly WarehouseContext db;

        public InventoryRepository(WarehouseContext db)
        {
            this.db = db;
        }

        public Task<List<InventoryItem>> GetAllAsync()
        {
            return db.InventoryItems
                .Include(i => i.Location)
                .OrderBy(i => i.Sku)
                .ToListAsync();
        }

        public Task<InventoryItem> GetAsync(int id)
        {
            return db.InventoryItems
                .Include(i => i.Location)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public void Add(InventoryItem item)
        {
            db.InventoryItems.Add(item);
        }

        public void Remove(InventoryItem item)
        {
            db.InventoryItems.Remove(item);
        }

        public Task SaveAsync()
        {
            return db.SaveChangesAsync();
        }
    }
}
