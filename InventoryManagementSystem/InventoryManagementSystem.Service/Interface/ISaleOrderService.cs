using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface ISaleOrderService
    {
        Task<SaleOrder> GetSaleOrderByIdAsync(int id);
        Task<IEnumerable<SaleOrder>> GetAllSaleOrdersAsync();
        Task<SaleOrder> CreateSaleOrderAsync(SaleOrder saleOrder);
        Task<SaleOrder> UpdateSaleOrderAsync(SaleOrder saleOrder);
        Task<bool> DeleteSaleOrderAsync(int id);
    }
}