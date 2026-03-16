using AutoMapper;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class StoneRateRepository : IStoneRateRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StoneRateRepository> _logger;

        public StoneRateRepository(
            AppDbContext context,
            IMapper mapper,
            ILogger<StoneRateRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StoneRateHistory?> GetCurrentRateBySearchAsync(
            int? stoneId = null,
            decimal? carat = null,
            string? cut = null,
            string? color = null,
            string? clarity = null,
            string? grade = null)
        {
            _logger.LogInformation("Getting current stone rate for search criteria");

            var query = _context.StoneRateHistories
                .Where(s => s.StatusId == 1); // Active status

            if (stoneId.HasValue)
            {
                query = query.Where(s => s.StoneId == stoneId.Value);
            }

            if (carat.HasValue)
            {
                query = query.Where(s => s.Carat == carat.Value);
            }

            if (!string.IsNullOrEmpty(cut))
            {
                query = query.Where(s => s.Cut == cut);
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(s => s.Color == color);
            }

            if (!string.IsNullOrEmpty(clarity))
            {
                query = query.Where(s => s.Clarity == clarity);
            }

            if (!string.IsNullOrEmpty(grade))
            {
                query = query.Where(s => s.Grade == grade);
            }

            var latestRate = await query
                .Include(s => s.Stone)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No stone rate found for the given criteria");
                return null;
            }

            return _mapper.Map<StoneRateHistory>(latestRate);
        }

        public async Task<StoneRateHistory?> GetCurrentRateByStoneIdAsync(int stoneId)
        {
            _logger.LogInformation("Getting current rate for stone {StoneId}", stoneId);

            var latestRate = await _context.StoneRateHistories
                .Include(s => s.Stone)
                .Where(s => s.StoneId == stoneId && s.StatusId == 1)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No rate found for stone {StoneId}", stoneId);
                return null;
            }

            return _mapper.Map<StoneRateHistory>(latestRate);
        }

        public async Task<StoneRateHistory?> GetDiamondRateBy4CsAsync(decimal carat, string cut, string color, string clarity)
        {
            _logger.LogInformation("Getting diamond rate for {Carat}ct, {Cut}, {Color}, {Clarity}", carat, cut, color, clarity);

            var latestRate = await _context.StoneRateHistories
                .Include(s => s.Stone)
                .Where(s => s.StoneId == 1 && // Assuming Diamond has ID 1
                           s.Carat == carat &&
                           s.Cut == cut &&
                           s.Color == color &&
                           s.Clarity == clarity &&
                           s.StatusId == 1)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No diamond rate found for {Carat}ct, {Cut}, {Color}, {Clarity}", carat, cut, color, clarity);
                return null;
            }

            return _mapper.Map<StoneRateHistory>(latestRate);
        }

        public async Task<StoneRateHistory?> GetRateByIdAsync(int id)
        {
            var rateDb = await _context.StoneRateHistories.FindAsync(id);
            return _mapper.Map<StoneRateHistory>(rateDb);
        }

        public async Task<StoneRateHistory> AddAsync(StoneRateHistory entity)
        {
            _logger.LogInformation("Adding new stone rate for stone {StoneId}", entity.StoneId);

            var entityDb = _mapper.Map<StoneRateHistoryDb>(entity);
            _context.StoneRateHistories.Add(entityDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Stone rate added successfully with ID {Id}", entityDb.Id);

            return _mapper.Map<StoneRateHistory>(entityDb);
        }

        public async Task<StoneRateHistory> UpdateAsync(StoneRateHistory entity)
        {
            _logger.LogInformation("Updating stone rate ID {Id}", entity.Id);

            var entityDb = await _context.StoneRateHistories.FindAsync(entity.Id);
            if (entityDb == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entityDb.StoneId = entity.StoneId;
            entityDb.Carat = entity.Carat;
            entityDb.Cut = entity.Cut;
            entityDb.Color = entity.Color;
            entityDb.Clarity = entity.Clarity;
            entityDb.Grade = entity.Grade;
            entityDb.RatePerUnit = entity.RatePerUnit;
            entityDb.EffectiveDate = entity.EffectiveDate;
            entityDb.StatusId = entity.StatusId;
            entityDb.UpdatedBy = entity.UpdatedBy;
            entityDb.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Stone rate updated successfully with ID {Id}", entityDb.Id);

            return _mapper.Map<StoneRateHistory>(entityDb);
        }

        public async Task<IEnumerable<StoneRateHistory>> GetRateHistoryByStoneIdAsync(int stoneId)
        {
            _logger.LogInformation("Getting rate history for stone {StoneId}", stoneId);

            var historyDb = await _context.StoneRateHistories
                .Include(s => s.Stone)
                .Where(s => s.StoneId == stoneId)
                .OrderByDescending(s => s.EffectiveDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StoneRateHistory>>(historyDb);
        }

        public async Task<IEnumerable<StoneRateHistory>> GetAllCurrentRatesAsync()
        {
            _logger.LogInformation("Getting all current stone rates");

            // Get the latest rate for each unique stone + 4Cs combination
            var latestRatesDb = await _context.StoneRateHistories
                .Include(s => s.Stone)
                .Where(s => s.StatusId == 1)
                .GroupBy(s => new { s.StoneId, s.Carat, s.Cut, s.Color, s.Clarity, s.Grade })
                .Select(g => g.OrderByDescending(s => s.EffectiveDate).First())
                .ToListAsync();

            return _mapper.Map<IEnumerable<StoneRateHistory>>(latestRatesDb);
        }

        public async Task<decimal> GetLatestRatePerUnitAsync(
            int stoneId,
            decimal? carat = null,
            string? cut = null,
            string? color = null,
            string? clarity = null,
            string? grade = null)
        {
            _logger.LogInformation("Getting latest rate per unit for stone {StoneId}", stoneId);

            var query = _context.StoneRateHistories
                .Where(s => s.StoneId == stoneId && s.StatusId == 1);

            if (carat.HasValue)
            {
                query = query.Where(s => s.Carat == carat.Value);
            }

            if (!string.IsNullOrEmpty(cut))
            {
                query = query.Where(s => s.Cut == cut);
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(s => s.Color == color);
            }

            if (!string.IsNullOrEmpty(clarity))
            {
                query = query.Where(s => s.Clarity == clarity);
            }

            if (!string.IsNullOrEmpty(grade))
            {
                query = query.Where(s => s.Grade == grade);
            }

            var latestRate = await query
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No rate found for stone {StoneId}, returning 0", stoneId);
                return 0;
            }

            return latestRate.RatePerUnit;
        }

        public async Task<IEnumerable<StoneRateHistory>> GetDiamondRateCardAsync()
        {
            _logger.LogInformation("Getting diamond rate card (all 4Cs combinations)");

            // Assuming Diamond has ID 1
            var diamondRatesDb = await _context.StoneRateHistories
                .Include(s => s.Stone)
                .Where(s => s.StoneId == 1 && s.StatusId == 1)
                .GroupBy(s => new { s.Carat, s.Cut, s.Color, s.Clarity })
                .Select(g => g.OrderByDescending(s => s.EffectiveDate).First())
                .ToListAsync();

            return _mapper.Map<IEnumerable<StoneRateHistory>>(diamondRatesDb);
        }
    }
}
