using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Interface for sending real-time user notifications via SignalR
    /// </summary>
    public interface IUserNotificationService
    {
        /// <summary>
        /// Notify all connected clients about a new user registration
        /// </summary>
        /// <param name="user">The newly registered user</param>
        Task NotifyUserRegisteredAsync(User user);

        /// <summary>
        /// Notify a specific user about updates to their profile
        /// </summary>
        /// <param name="userId">The user ID to notify</param>
        /// <param name="message">The notification message</param>
        Task NotifyUserAsync(long userId, string message);

        /// <summary>
        /// Notify all users in a specific role
        /// </summary>
        /// <param name="roleName">The role name to notify</param>
        /// <param name="message">The notification message</param>
        Task NotifyRoleAsync(string roleName, string message);

        /// <summary>
        /// Notify all connected clients with a broadcast message
        /// </summary>
        /// <param name="message">The notification message</param>
        Task NotifyAllAsync(string message);

        /// <summary>
        /// Notify about user status change (e.g., online/offline)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="status">The new status</param>
        Task NotifyUserStatusChangeAsync(long userId, string status);
    }
}
