using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class SaleOrderService : ISaleOrderService
    {
        private readonly ISaleOrderRepository _saleOrderRepository;
        private readonly ISaleOrderNotificationService _notificationService;

        public SaleOrderService(
            ISaleOrderRepository saleOrderRepository,
            ISaleOrderNotificationService notificationService)
        {
            _saleOrderRepository = saleOrderRepository;
            _notificationService = notificationService;
        }

        public async Task<SaleOrder> GetSaleOrderByIdAsync(int id)
        {
            return await _saleOrderRepository.GetSaleOrderByIdAsync(id);
        }

        public async Task<IEnumerable<SaleOrder>> GetAllSaleOrdersAsync()
        {
            return await _saleOrderRepository.GetAllSaleOrdersAsync();
        }

        public async Task<SaleOrder> CreateSaleOrderAsync(SaleOrder saleOrder)
        {
            var createdOrder = await _saleOrderRepository.CreateSaleOrderAsync(saleOrder);
            
            // Send real-time notification
            await _notificationService.NotifySaleOrderCreatedAsync(createdOrder);
            
            return createdOrder;
        }

        public async Task<SaleOrder> UpdateSaleOrderAsync(SaleOrder saleOrder)
        {
            var updatedOrder = await _saleOrderRepository.UpdateSaleOrderAsync(saleOrder);
            
            // Send notification about the update
            await _notificationService.NotifySaleOrderStatusChangedAsync(updatedOrder.Id, "Updated");
            
            return updatedOrder;
        }

        public async Task<bool> DeleteSaleOrderAsync(int id)
        {
            var result = await _saleOrderRepository.DeleteSaleOrderAsync(id);
            
            if (result)
            {
                // Send deletion notification
                await _notificationService.NotifySaleOrderDeletedAsync(id);
            }
            
            return result;
        }
    }
}