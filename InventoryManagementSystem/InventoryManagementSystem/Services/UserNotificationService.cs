using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Hubs;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Services
{
    /// <summary>
    /// Implementation of user notification service using SignalR
    /// </summary>
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IHubContext<UserHub> _hubContext;
        private readonly ILogger<UserNotificationService> _logger;

        public UserNotificationService(IHubContext<UserHub> hubContext, ILogger<UserNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task NotifyUserRegisteredAsync(User user)
        {
            try
            {
                var notification = new
                {
                    Type = "UserRegistered",
                    UserId = user.Id,
                    UserName = user.Name,
                    Email = user.Email,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.All.SendAsync("UserRegistered", notification);
                _logger.LogInformation("Sent user registration notification for user {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user registration notification for user {UserId}", user.Id);
            }
        }

        /// <inheritdoc />
        public async Task NotifyUserAsync(long userId, string message)
        {
            try
            {
                var groupName = $"User_{userId}";
                var notification = new
                {
                    Type = "UserNotification",
                    UserId = userId,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent notification to user {UserId}: {Message}", userId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
            }
        }

        /// <inheritdoc />
        public async Task NotifyRoleAsync(string roleName, string message)
        {
            try
            {
                var groupName = $"Role_{roleName}";
                var notification = new
                {
                    Type = "RoleNotification",
                    Role = roleName,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent notification to role {Role}: {Message}", roleName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to role {Role}", roleName);
            }
        }

        /// <inheritdoc />
        public async Task NotifyAllAsync(string message)
        {
            try
            {
                var notification = new
                {
                    Type = "Broadcast",
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent broadcast notification: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send broadcast notification");
            }
        }

        /// <inheritdoc />
        public async Task NotifyUserStatusChangeAsync(long userId, string status)
        {
            try
            {
                var notification = new
                {
                    Type = "UserStatusChange",
                    UserId = userId,
                    Status = status,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.All.SendAsync("UserStatusChanged", notification);
                _logger.LogInformation("Sent user status change notification for user {UserId}: {Status}", userId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user status change notification for user {UserId}", userId);
            }
        }
    }
}
