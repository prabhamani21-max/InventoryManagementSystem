using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class StoneRateService : IStoneRateService
    {
        private readonly IStoneRateRepository _stoneRateRepository;
        private readonly ILogger<StoneRateService> _logger;
        private readonly ICurrentUser _currentUser;

        public StoneRateService(
            IStoneRateRepository stoneRateRepository,
            ILogger<StoneRateService> logger,
            ICurrentUser currentUser)
        {
            _stoneRateRepository = stoneRateRepository;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<StoneRateHistory?> GetCurrentRateBySearchAsync(
            int? stoneId = null,
            decimal? carat = null,
            string? cut = null,
            string? color = null,
            string? clarity = null,
            string? grade = null)
        {
            return await _stoneRateRepository.GetCurrentRateBySearchAsync(stoneId, carat, cut, color, clarity, grade);
        }

        public async Task<StoneRateHistory?> GetCurrentRateByStoneIdAsync(int stoneId)
        {
            return await _stoneRateRepository.GetCurrentRateByStoneIdAsync(stoneId);
        }

        public async Task<StoneRateHistory?> GetDiamondRateBy4CsAsync(decimal carat, string cut, string color, string clarity)
        {
            return await _stoneRateRepository.GetDiamondRateBy4CsAsync(carat, cut, color, clarity);
        }

        public async Task<StoneRateHistory?> GetRateByIdAsync(int id)
        {
            return await _stoneRateRepository.GetRateByIdAsync(id);
        }

        public async Task<StoneRateHistory> AddStoneRateAsync(StoneRateHistory entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            entity.StatusId = 1; // Active status

            return await _stoneRateRepository.AddAsync(entity);
        }

        public async Task<StoneRateHistory> UpdateStoneRateAsync(StoneRateHistory entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            return await _stoneRateRepository.UpdateAsync(entity);
        }

        public async Task<IEnumerable<StoneRateHistory>> GetRateHistoryByStoneIdAsync(int stoneId)
        {
            return await _stoneRateRepository.GetRateHistoryByStoneIdAsync(stoneId);
        }

        public async Task<IEnumerable<StoneRateHistory>> GetAllCurrentRatesAsync()
        {
            return await _stoneRateRepository.GetAllCurrentRatesAsync();
        }

        public async Task<decimal> GetLatestRatePerUnitAsync(int stoneId, decimal? carat = null, string? cut = null, string? color = null, string? clarity = null, string? grade = null)
        {
            return await _stoneRateRepository.GetLatestRatePerUnitAsync(stoneId, carat, cut, color, clarity, grade);
        }

        public async Task<IEnumerable<StoneRateHistory>> GetDiamondRateCardAsync()
        {
            return await _stoneRateRepository.GetDiamondRateCardAsync();
        }
    }
}
