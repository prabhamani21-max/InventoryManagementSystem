using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Keeps invoice payment allocations and balances synchronized with order payments.
    /// </summary>
    public interface IInvoiceSettlementService
    {
        Task<Invoice?> GetActiveSaleInvoiceAsync(long saleOrderId);
        Task<Invoice?> RefreshSaleInvoicePaymentsAsync(long saleOrderId);
    }
}
