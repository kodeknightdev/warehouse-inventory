using System;
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

        public Task<List<string>> GetCategoriesAsync()
        {
            return db.InventoryItems
                .Where(i => i.Category != null)
                .Select(i => i.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<Tuple<List<InventoryItem>, int>> SearchAsync(string search, string category, int? locationId, int page, int pageSize)
        {
            var query = db.InventoryItems.Include(i => i.Location);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(i => i.Sku.Contains(term) || i.Name.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(i => i.Category == category);

            if (locationId.HasValue)
                query = query.Where(i => i.LocationId == locationId);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.Sku)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Tuple.Create(items, total);
        }

        public Task<InventoryItem> GetAsync(int id)
        {
            return db.InventoryItems
                .Include(i => i.Location)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<bool> SkuExistsAsync(string sku, int? exceptId = null)
        {
            return db.InventoryItems.AnyAsync(i => i.Sku == sku && (exceptId == null || i.Id != exceptId));
        }

        public void Add(InventoryItem item)
        {
            db.InventoryItems.Add(item);
        }

        public Task<List<StockMovement>> GetMovementsAsync(int itemId, int take)
        {
            return db.StockMovements
                .Where(m => m.InventoryItemId == itemId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public void AddMovement(StockMovement movement)
        {
            db.StockMovements.Add(movement);
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
