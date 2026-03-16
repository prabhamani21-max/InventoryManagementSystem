using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            return await _supplierRepository.GetSupplierByIdAsync(id);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _supplierRepository.GetAllSuppliersAsync();
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            return await _supplierRepository.CreateSupplierAsync(supplier);
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            return await _supplierRepository.UpdateSupplierAsync(supplier);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            return await _supplierRepository.DeleteSupplierAsync(id);
        }

        public async Task<bool> GSTNumberExistsAsync(string gstNumber, int? excludeId = null)
        {
            return await _supplierRepository.GSTNumberExistsAsync(gstNumber, excludeId);
        }

        public async Task<bool> TANNumberExistsAsync(string tanNumber, int? excludeId = null)
        {
            return await _supplierRepository.TANNumberExistsAsync(tanNumber, excludeId);
        }
    }
}