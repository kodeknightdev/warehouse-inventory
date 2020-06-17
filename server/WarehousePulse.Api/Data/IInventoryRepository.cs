using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Data
{
    public interface IInventoryRepository
    {
        Task<List<string>> GetCategoriesAsync();
        Task<Tuple<List<InventoryItem>, int>> SearchAsync(string search, string category, int? locationId, int page, int pageSize);
        Task<InventoryItem> GetAsync(int id);
        Task<bool> SkuExistsAsync(string sku, int? exceptId = null);
        void Add(InventoryItem item);
        Task<List<StockMovement>> GetMovementsAsync(int itemId, int take);
        void AddMovement(StockMovement movement);
        void Remove(InventoryItem item);
        Task SaveAsync();
    }
}
