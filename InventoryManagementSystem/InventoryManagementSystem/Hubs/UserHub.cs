using Microsoft.AspNetCore.SignalR;

namespace InventoryManagementSystem.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time user notifications
    /// </summary>
    public class UserHub : Hub
    {
        private readonly ILogger<UserHub> _logger;

        public UserHub(ILogger<UserHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation("Client connected: {ConnectionId}", connectionId);
            
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
                _logger.LogWarning(exception, "Client disconnected with error: {ConnectionId}", connectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected: {ConnectionId}", connectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a specific user group for targeted notifications
        /// </summary>
        /// <param name="userId">The user ID to join the group for</param>
        public async Task JoinUserGroup(long userId)
        {
            var groupName = $"User_{userId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Leave a specific user group
        /// </summary>
        /// <param name="userId">The user ID to leave the group for</param>
        public async Task LeaveUserGroup(long userId)
        {
            var groupName = $"User_{userId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Join a role-based group for role-specific notifications
        /// </summary>
        /// <param name="roleName">The role name to join the group for</param>
        public async Task JoinRoleGroup(string roleName)
        {
            var groupName = $"Role_{roleName}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Leave a role-based group
        /// </summary>
        /// <param name="roleName">The role name to leave the group for</param>
        public async Task LeaveRoleGroup(string roleName)
        {
            var groupName = $"Role_{roleName}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
        }
    }
}
