using Microsoft.AspNetCore.SignalR;

namespace InventoryManagementSystem.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time sale order notifications
    /// </summary>
    public class SaleOrderHub : Hub
    {
        private readonly ILogger<SaleOrderHub> _logger;

        public SaleOrderHub(ILogger<SaleOrderHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation("Client connected to SaleOrderHub: {ConnectionId}", connectionId);
            
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        /// <param name="exception">Exception that caused the disconnection, if any</param>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected from SaleOrderHub with error: {ConnectionId}", connectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected from SaleOrderHub: {ConnectionId}", connectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a specific sale order group for targeted notifications
        /// </summary>
        /// <param name="saleOrderId">The sale order ID to join the group for</param>
        public async Task JoinSaleOrderGroup(long saleOrderId)
        {
            var groupName = $"SaleOrder_{saleOrderId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Leave a specific sale order group
        /// </summary>
        /// <param name="saleOrderId">The sale order ID to leave the group for</param>
        public async Task LeaveSaleOrderGroup(long saleOrderId)
        {
            var groupName = $"SaleOrder_{saleOrderId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Join a customer-specific group for all their sale order notifications
        /// </summary>
        /// <param name="customerId">The customer ID to join the group for</param>
        public async Task JoinCustomerGroup(long customerId)
        {
            var groupName = $"Customer_{customerId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined customer group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Leave a customer-specific group
        /// </summary>
        /// <param name="customerId">The customer ID to leave the group for</param>
        public async Task LeaveCustomerGroup(long customerId)
        {
            var groupName = $"Customer_{customerId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left customer group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Join the sales team group for all sale order notifications
        /// </summary>
        public async Task JoinSalesTeamGroup()
        {
            const string groupName = "SalesTeam";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined SalesTeam group", Context.ConnectionId);
        }

        /// <summary>
        /// Leave the sales team group
        /// </summary>
        public async Task LeaveSalesTeamGroup()
        {
            const string groupName = "SalesTeam";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left SalesTeam group", Context.ConnectionId);
        }
    }
}
