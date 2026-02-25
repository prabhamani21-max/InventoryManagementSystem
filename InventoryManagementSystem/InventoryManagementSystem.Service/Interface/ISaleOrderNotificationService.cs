using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Interface for sending real-time sale order notifications via SignalR
    /// </summary>
    public interface ISaleOrderNotificationService
    {
        /// <summary>
        /// Notify all connected clients about a new sale order creation
        /// </summary>
        /// <param name="saleOrder">The newly created sale order</param>
        Task NotifySaleOrderCreatedAsync(SaleOrder saleOrder);

        /// <summary>
        /// Notify about sale order status update
        /// </summary>
        /// <param name="saleOrderId">The sale order ID</param>
        /// <param name="status">The new status</param>
        Task NotifySaleOrderStatusChangedAsync(long saleOrderId, string status);

        /// <summary>
        /// Notify a specific customer about their sale order update
        /// </summary>
        /// <param name="customerId">The customer ID to notify</param>
        /// <param name="message">The notification message</param>
        Task NotifyCustomerAsync(long customerId, string message);

        /// <summary>
        /// Notify about sale order deletion
        /// </summary>
        /// <param name="saleOrderId">The deleted sale order ID</param>
        Task NotifySaleOrderDeletedAsync(long saleOrderId);

        /// <summary>
        /// Notify about sale order delivery date update
        /// </summary>
        /// <param name="saleOrderId">The sale order ID</param>
        /// <param name="deliveryDate">The new delivery date</param>
        Task NotifyDeliveryDateUpdatedAsync(long saleOrderId, DateTime? deliveryDate);
    }
}
