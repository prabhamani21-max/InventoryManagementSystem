using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class UserKycService : IUserKycService
    {
        private readonly IUserKycRepository _userKycRepository;

        public UserKycService(IUserKycRepository userKycRepository)
        {
            _userKycRepository = userKycRepository;
        }

        public async Task<UserKyc> GetUserKycByIdAsync(long id)
        {
            return await _userKycRepository.GetUserKycByIdAsync(id);
        }

        public async Task<UserKyc> GetUserKycByUserIdAsync(long userId)
        {
            return await _userKycRepository.GetUserKycByUserIdAsync(userId);
        }

        public async Task<UserKyc> CreateUserKycAsync(UserKyc userKyc)
        {
            return await _userKycRepository.CreateUserKycAsync(userKyc);
        }

        public async Task<UserKyc> UpdateUserKycAsync(UserKyc userKyc)
        {
            return await _userKycRepository.UpdateUserKycAsync(userKyc);
        }

        public async Task<bool> DeleteUserKycAsync(long id)
        {
            return await _userKycRepository.DeleteUserKycAsync(id);
        }

        public async Task<IEnumerable<UserKyc>> GetAllUserKycsAsync()
        {
            return await _userKycRepository.GetAllUserKycsAsync();
        }
        public async Task<bool> UserKycExistsAsync(long userId, long? excludeId = null)
        {
            return await _userKycRepository.UserKycExistsAsync(userId);
        }

        public async Task<bool> PanCardNumberExistsAsync(string panCardNumber, long? excludeId = null)
        {
            return await _userKycRepository.PanCardNumberExistsAsync(panCardNumber, excludeId);
        }

        public async Task<bool> AadhaarCardNumberExistsAsync(string aadhaarCardNumber, long? excludeId = null)
        {
            return await _userKycRepository.AadhaarCardNumberExistsAsync(aadhaarCardNumber, excludeId);
        }
    }
}