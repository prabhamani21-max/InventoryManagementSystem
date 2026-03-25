using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface ISaleOrderRepository
    {
        Task<SaleOrder> GetSaleOrderByIdAsync(long id);
        Task<SaleOrder?> GetSaleOrderByExchangeOrderIdAsync(long exchangeOrderId);
        Task<IEnumerable<SaleOrder>> GetAllSaleOrdersAsync();
        Task<SaleOrder> CreateSaleOrderAsync(SaleOrder saleOrder);
        Task<SaleOrder> UpdateSaleOrderAsync(SaleOrder saleOrder);
        Task<bool> DeleteSaleOrderAsync(long id);
        
        /// <summary>
        /// Get all sale orders for a specific customer
        /// </summary>
        /// <param name="customerId">The customer's user ID</param>
        /// <returns>List of sale orders for the customer</returns>
        Task<IEnumerable<SaleOrder>> GetSaleOrdersByCustomerIdAsync(long customerId);

        /// <summary>
        /// Get all sale orders created by a specific sales person
        /// </summary>
        /// <param name="salespersonId">The sales person's user ID</param>
        /// <returns>List of sale orders created by the sales person</returns>
        Task<IEnumerable<SaleOrder>> GetSaleOrdersBySalesPersonAsync(long salesPersonId);
    }
}
