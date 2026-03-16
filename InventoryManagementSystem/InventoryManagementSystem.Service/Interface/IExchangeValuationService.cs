using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IExchangeValuationService
    {
        Task<ExchangeCalculationResult> CalculateAsync(ExchangeCalculationRequest request);
    }
}
