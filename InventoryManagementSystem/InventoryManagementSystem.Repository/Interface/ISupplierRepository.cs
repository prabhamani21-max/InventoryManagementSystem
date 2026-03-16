using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface ISupplierRepository
    {
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);
        Task<bool> GSTNumberExistsAsync(string gstNumber, int? excludeId = null);
        Task<bool> TANNumberExistsAsync(string tanNumber, int? excludeId = null);
    }
}