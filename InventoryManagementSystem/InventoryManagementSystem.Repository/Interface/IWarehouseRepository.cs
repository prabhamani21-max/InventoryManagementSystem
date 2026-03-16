using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IWarehouseRepository
    {
        Task<Warehouse> GetWarehouseByIdAsync(int id);
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
        Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse);
        Task<bool> DeleteWarehouseAsync(int id);
    }
}