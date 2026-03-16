using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseService(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int id)
        {
            return await _warehouseRepository.GetWarehouseByIdAsync(id);
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _warehouseRepository.GetAllWarehousesAsync();
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            return await _warehouseRepository.CreateWarehouseAsync(warehouse);
        }

        public async Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse)
        {
            return await _warehouseRepository.UpdateWarehouseAsync(warehouse);
        }

        public async Task<bool> DeleteWarehouseAsync(int id)
        {
            return await _warehouseRepository.DeleteWarehouseAsync(id);
        }
    }
}