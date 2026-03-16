using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Hubs;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Services
{
    /// <summary>
    /// Implementation of sale order notification service using SignalR
    /// </summary>
    public class SaleOrderNotificationService : ISaleOrderNotificationService
    {
        private readonly IHubContext<SaleOrderHub> _hubContext;
        private readonly ILogger<SaleOrderNotificationService> _logger;

        public SaleOrderNotificationService(IHubContext<SaleOrderHub> hubContext, ILogger<SaleOrderNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task NotifySaleOrderCreatedAsync(SaleOrder saleOrder)
        {
            try
            {
                var notification = new
                {
                    Type = "SaleOrderCreated",
                    SaleOrderId = saleOrder.Id,
                    OrderNumber = saleOrder.OrderNumber,
                    CustomerId = saleOrder.CustomerId,
                    OrderDate = saleOrder.OrderDate,
                    DeliveryDate = saleOrder.DeliveryDate,
                    IsExchangeSale = saleOrder.IsExchangeSale,
                    Timestamp = DateTime.UtcNow
                };

                // Notify all clients in SalesTeam group
                await _hubContext.Clients.Group("SalesTeam").SendAsync("SaleOrderCreated", notification);
                
                // Also notify the specific customer
                var customerGroup = $"Customer_{saleOrder.CustomerId}";
                await _hubContext.Clients.Group(customerGroup).SendAsync("SaleOrderCreated", notification);
                
                _logger.LogInformation("Sent sale order creation notification for Order {OrderNumber}", saleOrder.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send sale order creation notification for Order {OrderNumber}", saleOrder.OrderNumber);
            }
        }

        /// <inheritdoc />
        public async Task NotifySaleOrderStatusChangedAsync(long saleOrderId, string status)
        {
            try
            {
                var notification = new
                {
                    Type = "SaleOrderStatusChanged",
                    SaleOrderId = saleOrderId,
                    Status = status,
                    Timestamp = DateTime.UtcNow
                };

                // Notify the specific sale order group
                var saleOrderGroup = $"SaleOrder_{saleOrderId}";
                await _hubContext.Clients.Group(saleOrderGroup).SendAsync("SaleOrderStatusChanged", notification);
                
                // Also notify SalesTeam
                await _hubContext.Clients.Group("SalesTeam").SendAsync("SaleOrderStatusChanged", notification);
                
                _logger.LogInformation("Sent sale order status change notification for Order {SaleOrderId}: {Status}", saleOrderId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send sale order status change notification for Order {SaleOrderId}", saleOrderId);
            }
        }

        /// <inheritdoc />
        public async Task NotifyCustomerAsync(long customerId, string message)
        {
            try
            {
                var notification = new
                {
                    Type = "CustomerNotification",
                    CustomerId = customerId,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                var customerGroup = $"Customer_{customerId}";
                await _hubContext.Clients.Group(customerGroup).SendAsync("ReceiveNotification", notification);
                
                _logger.LogInformation("Sent notification to customer {CustomerId}: {Message}", customerId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to customer {CustomerId}", customerId);
            }
        }

        /// <inheritdoc />
        public async Task NotifySaleOrderDeletedAsync(long saleOrderId)
        {
            try
            {
                var notification = new
                {
                    Type = "SaleOrderDeleted",
                    SaleOrderId = saleOrderId,
                    Timestamp = DateTime.UtcNow
                };

                // Notify SalesTeam about deletion
                await _hubContext.Clients.Group("SalesTeam").SendAsync("SaleOrderDeleted", notification);
                
                _logger.LogInformation("Sent sale order deletion notification for Order {SaleOrderId}", saleOrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send sale order deletion notification for Order {SaleOrderId}", saleOrderId);
            }
        }

        /// <inheritdoc />
        public async Task NotifyDeliveryDateUpdatedAsync(long saleOrderId, DateTime? deliveryDate)
        {
            try
            {
                var notification = new
                {
                    Type = "DeliveryDateUpdated",
                    SaleOrderId = saleOrderId,
                    DeliveryDate = deliveryDate,
                    Timestamp = DateTime.UtcNow
                };

                // Notify the specific sale order group
                var saleOrderGroup = $"SaleOrder_{saleOrderId}";
                await _hubContext.Clients.Group(saleOrderGroup).SendAsync("DeliveryDateUpdated", notification);
                
                // Also notify SalesTeam
                await _hubContext.Clients.Group("SalesTeam").SendAsync("DeliveryDateUpdated", notification);
                
                _logger.LogInformation("Sent delivery date update notification for Order {SaleOrderId}", saleOrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send delivery date update notification for Order {SaleOrderId}", saleOrderId);
            }
        }
    }
}
