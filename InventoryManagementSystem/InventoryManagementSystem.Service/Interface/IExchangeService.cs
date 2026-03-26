using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IExchangeService
    {
        /// <summary>
        /// Calculate exchange value without creating order (preview)
        /// </summary>
        Task<ExchangeCalculationResult> CalculateExchangeValueAsync(ExchangeCalculationRequest request);

        /// <summary>
        /// Create new exchange/buyback order
        /// </summary>
        Task<ExchangeOrder> CreateExchangeOrderAsync(ExchangeOrder exchangeOrder);

        /// <summary>
        /// Get exchange order by ID
        /// </summary>
        Task<ExchangeOrder?> GetExchangeOrderByIdAsync(long id);

        /// <summary>
        /// Get exchange order by order number
        /// </summary>
        Task<ExchangeOrder?> GetExchangeOrderByOrderNumberAsync(string orderNumber);

        /// <summary>
        /// Get all exchange orders for a customer
        /// </summary>
        Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCustomerIdAsync(long customerId);

        /// <summary>
        /// Get all exchange orders created by a specific sales person
        /// </summary>
        /// <param name="createdBy">The sales person's user ID</param>
        /// <returns>List of exchange orders created by the sales person</returns>
        Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCreatedByAsync(long createdBy);

        /// <summary>
        /// Get all exchange orders
        /// </summary>
        Task<IEnumerable<ExchangeOrder>> GetAllExchangeOrdersAsync();

        /// <summary>
        /// Link a sale order to an exchange order for Phase 1 exchange settlement.
        /// </summary>
        Task<ExchangeOrder> LinkSaleOrderAsync(long exchangeOrderId, long saleOrderId);

        /// <summary>
        /// Complete exchange order after validating the linked sale and invoice.
        /// </summary>
        Task<ExchangeOrder> CompleteExchangeOrderAsync(long orderId, string? notes);

        /// <summary>
        /// Cancel exchange order
        /// </summary>
        Task<bool> CancelExchangeOrderAsync(long orderId, string? reason);
    }
}
