using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IPaymentRepository
    {
        Task<Payment> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task<bool> DeletePaymentAsync(int id);

        /// <summary>
        /// Gets payments by order ID and order type
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="orderType">The order type (SALE, PURCHASE, etc.)</param>
        /// <returns>List of payment DB models</returns>
        Task<List<PaymentDb>> GetPaymentsByOrderIdAndTypeAsync(long orderId, TransactionType orderType);
    }
}