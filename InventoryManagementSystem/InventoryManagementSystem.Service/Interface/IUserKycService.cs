using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IUserKycService
    {
        Task<UserKyc> GetUserKycByIdAsync(long id);
        Task<UserKyc> GetUserKycByUserIdAsync(long userId);
        Task<UserKyc> CreateUserKycAsync(UserKyc userKyc);
        Task<UserKyc> UpdateUserKycAsync(UserKyc userKyc);
        Task<bool> DeleteUserKycAsync(long id);
        Task<IEnumerable<UserKyc>> GetAllUserKycsAsync();
        Task<bool> PanCardNumberExistsAsync(string panCardNumber, long? excludeId = null);
        Task<bool> AadhaarCardNumberExistsAsync(string aadhaarCardNumber, long? excludeId = null);
        Task<bool> UserKycExistsAsync(long userId, long? excludeId = null);
        
        /// <summary>
        /// Checks if a user's KYC is verified
        /// </summary>
        /// <param name="userId">The user ID to check</param>
        /// <returns>True if KYC exists and is verified, false otherwise</returns>
        Task<bool> IsUserKycVerifiedAsync(long userId);
    }
}