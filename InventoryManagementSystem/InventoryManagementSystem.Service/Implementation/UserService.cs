using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;


namespace InventoryManagementSystem.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationService _notificationService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IUserNotificationService notificationService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

    

         public async Task<User> RegisterUserAsync(User user)
        {
            var registeredUser = await _userRepository.RegisterUserAsync(user);
            
            // Send real-time notification about new user registration
            try
            {
                await _notificationService.NotifyUserRegisteredAsync(registeredUser);
                _logger.LogInformation("User registered successfully with ID {UserId}. Notification sent.", registeredUser.Id);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the registration
                _logger.LogError(ex, "Failed to send registration notification for user {UserId}", registeredUser.Id);
            }
            
            return registeredUser;
        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }
        public async Task<UserDb> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<UserDb> GetUserByContactNumberAsync(string contactNumber)
        {
            return await _userRepository.GetUserByContactNumberAsync(contactNumber);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<bool> ContactNumberExistsAsync(string contactNo)
        {
            return await _userRepository.ContactNumberExistsAsync(contactNo);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        
        }
    }
}