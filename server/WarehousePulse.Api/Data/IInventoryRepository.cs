using System.Collections.Generic;
using System.Threading.Tasks;
using WarehousePulse.Api.Models;

namespace WarehousePulse.Api.Data
{
    public interface IInventoryRepository
    {
        Task<List<InventoryItem>> GetAllAsync();
        Task<InventoryItem> GetAsync(int id);
        Task<bool> SkuExistsAsync(string sku, int? exceptId = null);
        void Add(InventoryItem item);
        void Remove(InventoryItem item);
        Task SaveAsync();
    }
}
