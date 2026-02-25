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
        
        // Exchange calculation methods
        Task<ExchangeCalculationResult> CalculateExchangeValueAsync(ExchangeCalculationRequest request);
        Task<int> GetStatusIdByNameAsync(string statusName);
    }
}
