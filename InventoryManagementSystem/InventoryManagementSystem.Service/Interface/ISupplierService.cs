using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface ISupplierService
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