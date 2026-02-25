using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface ISaleOrderRepository
    {
        Task<SaleOrder> GetSaleOrderByIdAsync(int id);
        Task<IEnumerable<SaleOrder>> GetAllSaleOrdersAsync();
        Task<SaleOrder> CreateSaleOrderAsync(SaleOrder saleOrder);
        Task<SaleOrder> UpdateSaleOrderAsync(SaleOrder saleOrder);
        Task<bool> DeleteSaleOrderAsync(int id);
    }
}