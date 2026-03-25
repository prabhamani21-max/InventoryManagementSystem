using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class SaleOrderService : ISaleOrderService
    {
        private readonly ISaleOrderRepository _saleOrderRepository;
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ISaleOrderNotificationService _notificationService;

        public SaleOrderService(
            ISaleOrderRepository saleOrderRepository,
            IExchangeRepository exchangeRepository,
            ISaleOrderNotificationService notificationService)
        {
            _saleOrderRepository = saleOrderRepository;
            _exchangeRepository = exchangeRepository;
            _notificationService = notificationService;
        }

        public async Task<SaleOrder> GetSaleOrderByIdAsync(long id)
        {
            return await _saleOrderRepository.GetSaleOrderByIdAsync(id);
        }

        public async Task<SaleOrder?> GetSaleOrderByExchangeOrderIdAsync(long exchangeOrderId)
        {
            return await _saleOrderRepository.GetSaleOrderByExchangeOrderIdAsync(exchangeOrderId);
        }

        public async Task<IEnumerable<SaleOrder>> GetAllSaleOrdersAsync()
        {
            return await _saleOrderRepository.GetAllSaleOrdersAsync();
        }

        public async Task<SaleOrder> CreateSaleOrderAsync(SaleOrder saleOrder)
        {
            NormalizeExchangeFlags(saleOrder);
            await EnsureExchangeSaleLinkIsAvailableAsync(saleOrder);

            var createdOrder = await _saleOrderRepository.CreateSaleOrderAsync(saleOrder);
            
            // Send real-time notification
            await _notificationService.NotifySaleOrderCreatedAsync(createdOrder);
            
            return createdOrder;
        }

        public async Task<SaleOrder> UpdateSaleOrderAsync(SaleOrder saleOrder)
        {
            NormalizeExchangeFlags(saleOrder);
            await EnsureExchangeSaleLinkIsAvailableAsync(saleOrder);

            var updatedOrder = await _saleOrderRepository.UpdateSaleOrderAsync(saleOrder);
            
            // Send notification about the update
            await _notificationService.NotifySaleOrderStatusChangedAsync(updatedOrder.Id, "Updated");
            
            return updatedOrder;
        }

        public async Task<bool> DeleteSaleOrderAsync(long id)
        {
            var result = await _saleOrderRepository.DeleteSaleOrderAsync(id);
            
            if (result)
            {
                // Send deletion notification
                await _notificationService.NotifySaleOrderDeletedAsync(id);
            }
            
            return result;
        }

        /// <summary>
        /// Get all sale orders for a specific customer
        /// </summary>
        /// <param name="customerId">The customer's user ID</param>
        /// <returns>List of sale orders for the customer</returns>
        public async Task<IEnumerable<SaleOrder>> GetSaleOrdersByCustomerIdAsync(long customerId)
        {
            return await _saleOrderRepository.GetSaleOrdersByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Get all sale orders created by a specific sales person
        /// </summary>
        /// <param name="salespersonId">The sales person's user ID</param>
        /// <returns>List of sale orders created by the sales person</returns>
        public async Task<IEnumerable<SaleOrder>> GetSaleOrdersBySalesPersonAsync(long salesPersonId)
        {
            return await _saleOrderRepository.GetSaleOrdersBySalesPersonAsync(salesPersonId);
        }

        private static void NormalizeExchangeFlags(SaleOrder saleOrder)
        {
            if (saleOrder.ExchangeOrderId.HasValue)
            {
                saleOrder.IsExchangeSale = true;
            }

            if (saleOrder.IsExchangeSale && !saleOrder.ExchangeOrderId.HasValue)
            {
                throw new InvalidOperationException("Exchange sale orders must reference an exchange order.");
            }
        }

        private async Task EnsureExchangeSaleLinkIsAvailableAsync(SaleOrder saleOrder)
        {
            if (!saleOrder.ExchangeOrderId.HasValue)
            {
                return;
            }

            var existingLinkedSale = await _saleOrderRepository.GetSaleOrderByExchangeOrderIdAsync(saleOrder.ExchangeOrderId.Value);
            if (existingLinkedSale != null && existingLinkedSale.Id != saleOrder.Id)
            {
                throw new InvalidOperationException(
                    $"Exchange order {saleOrder.ExchangeOrderId.Value} is already linked to sale order {existingLinkedSale.Id}.");
            }

            // Validate that the exchange order belongs to the same customer as the sale order
            var exchangeOrder = await _exchangeRepository.GetExchangeOrderByIdAsync(saleOrder.ExchangeOrderId.Value);
            if (exchangeOrder == null)
            {
                throw new InvalidOperationException($"Exchange order {saleOrder.ExchangeOrderId.Value} not found.");
            }

            if (exchangeOrder.CustomerId != saleOrder.CustomerId)
            {
                throw new InvalidOperationException(
                    $"Exchange order {saleOrder.ExchangeOrderId.Value} belongs to customer {exchangeOrder.CustomerId} but sale order is for customer {saleOrder.CustomerId}. Exchange orders can only be used in sales for the same customer.");
            }
        }
    }
}
