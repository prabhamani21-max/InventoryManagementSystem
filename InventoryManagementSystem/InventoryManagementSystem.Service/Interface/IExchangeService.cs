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
        /// Get all exchange orders
        /// </summary>
        Task<IEnumerable<ExchangeOrder>> GetAllExchangeOrdersAsync();

        /// <summary>
        /// Complete exchange order (add old gold to inventory, apply credit)
        /// </summary>
        Task<ExchangeOrder> CompleteExchangeOrderAsync(long orderId, string? notes);

        /// <summary>
        /// Cancel exchange order
        /// </summary>
        Task<bool> CancelExchangeOrderAsync(long orderId, string? reason);
    }
}
