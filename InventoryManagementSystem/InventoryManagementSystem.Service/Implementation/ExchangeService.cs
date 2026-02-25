using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<ExchangeService> _logger;
        private readonly IMapper _mapper;

        public ExchangeService(
            IExchangeRepository exchangeRepository,
            ICurrentUser currentUser,
            ILogger<ExchangeService> logger,
            IMapper mapper)
        {
            _exchangeRepository = exchangeRepository;
            _currentUser = currentUser;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ExchangeCalculationResult> CalculateExchangeValueAsync(ExchangeCalculationRequest request)
        {
            _logger.LogInformation("Calculating exchange value for customer {CustomerId}", request.CustomerId);
            return await _exchangeRepository.CalculateExchangeValueAsync(request);
        }

        public async Task<ExchangeOrder> CreateExchangeOrderAsync(ExchangeOrder request)
        {
            _logger.LogInformation("Creating exchange order for customer {CustomerId}", request.CustomerId);

            // Generate order number
            var orderNumber = $"EXC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            request.OrderNumber = orderNumber;

            // Set audit fields
            request.ExchangeDate = DateTime.UtcNow;
            request.CreatedDate = DateTime.UtcNow;
            request.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            
            // Set default status to PENDING
            if (request.StatusId == 0)
            {
                request.StatusId = await _exchangeRepository.GetStatusIdByNameAsync("PENDING");
            }

            // Set audit fields for items
            foreach (var item in request.Items)
            {
                item.CreatedDate = DateTime.UtcNow;
                item.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
                if (item.StatusId == 0)
                {
                    item.StatusId = request.StatusId;
                }
            }

            var createdOrder = await _exchangeRepository.CreateExchangeOrderAsync(request);
            _logger.LogInformation("Exchange order {OrderNumber} created successfully", orderNumber);

            return createdOrder;
        }

        public async Task<ExchangeOrder?> GetExchangeOrderByIdAsync(long id)
        {
            var order = await _exchangeRepository.GetExchangeOrderByIdAsync(id);
            return order;
        }

        public async Task<ExchangeOrder?> GetExchangeOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await _exchangeRepository.GetExchangeOrderByOrderNumberAsync(orderNumber);
            return order;
        }

        public async Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCustomerIdAsync(long customerId)
        {
            var orders = await _exchangeRepository.GetExchangeOrdersByCustomerIdAsync(customerId);
            return orders;
        }

        public async Task<IEnumerable<ExchangeOrder>> GetAllExchangeOrdersAsync()
        {
            var orders = await _exchangeRepository.GetAllExchangeOrdersAsync();
            return orders;
        }

        public async Task<ExchangeOrder> CompleteExchangeOrderAsync(long orderId, string? notes)
        {
            _logger.LogInformation("Completing exchange order {OrderId}", orderId);

            var order = await _exchangeRepository.GetExchangeOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Exchange order {orderId} not found");
            }

            // Check if order is in correct status
            if (order.StatusId != await _exchangeRepository.GetStatusIdByNameAsync("PENDING"))
            {
                throw new InvalidOperationException("Exchange order cannot be completed. Only PENDING orders can be completed.");
            }

            // Get completed status id
            var completedStatusId = await _exchangeRepository.GetStatusIdByNameAsync("COMPLETED");

            // Update order status
            order.StatusId = completedStatusId;
            order.UpdatedDate = DateTime.UtcNow;
            order.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            order.Notes = notes ?? order.Notes;

            // Update items status
            foreach (var item in order.Items)
            {
                item.StatusId = completedStatusId;
            }

            var updatedOrder = await _exchangeRepository.UpdateExchangeOrderAsync(order);
            _logger.LogInformation("Exchange order {OrderId} completed successfully", orderId);

            return updatedOrder;
        }

        public async Task<bool> CancelExchangeOrderAsync(long orderId, string? reason)
        {
            _logger.LogInformation("Cancelling exchange order {OrderId}", orderId);

            var order = await _exchangeRepository.GetExchangeOrderByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            // Check if order can be cancelled
            if (order.StatusId == await _exchangeRepository.GetStatusIdByNameAsync("COMPLETED") || 
                order.StatusId == await _exchangeRepository.GetStatusIdByNameAsync("CANCELLED"))
            {
                throw new InvalidOperationException("Exchange order cannot be cancelled in its current status.");
            }

            var result = await _exchangeRepository.CancelExchangeOrderAsync(orderId);
            if (result)
            {
                _logger.LogInformation("Exchange order {OrderId} cancelled successfully", orderId);
            }

            return result;
        }
    }
}
