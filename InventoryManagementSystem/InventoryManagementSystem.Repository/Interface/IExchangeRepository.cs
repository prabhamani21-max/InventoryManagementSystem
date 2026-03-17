using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IExchangeRepository
    {
        Task<ExchangeOrder?> GetExchangeOrderByIdAsync(long id);
        Task<ExchangeOrder?> GetExchangeOrderByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCustomerIdAsync(long customerId);
        Task<IEnumerable<ExchangeOrder>> GetAllExchangeOrdersAsync();
        Task<ExchangeOrder> CreateExchangeOrderAsync(ExchangeOrder exchangeOrder);
        Task<ExchangeOrder> UpdateExchangeOrderAsync(ExchangeOrder exchangeOrder);
        Task<bool> CancelExchangeOrderAsync(long orderId);

        /// <summary>
        /// Get all exchange orders created by a specific sales person
        /// </summary>
        /// <param name="createdBy">The sales person's user ID</param>
        /// <returns>List of exchange orders created by the sales person</returns>
        Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCreatedByAsync(long createdBy);

        Task<(decimal RatePerGram, decimal PurityPercentage)> GetCurrentRateWithPurityAsync(int purityId);
    }
}
