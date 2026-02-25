using AutoMapper;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class MetalRateRepository : IMetalRateRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MetalRateRepository> _logger;

        public MetalRateRepository(AppDbContext context, IMapper mapper, ILogger<MetalRateRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MetalRateHistory?> GetCurrentRateByPurityIdAsync(int purityId)
        {
            _logger.LogInformation("Getting current rate for purity {PurityId}", purityId);

            var latestRate = await _context.MetalRateHistories
                .Where(g => g.PurityId == purityId)
                .OrderByDescending(g => g.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No rate found for purity {PurityId}", purityId);
                return null;
            }

            return _mapper.Map<MetalRateHistory>(latestRate);
        }

        public async Task<MetalRateHistory?> GetRateByIdAsync(int id)
        {
            var rateDb = await _context.MetalRateHistories.FindAsync(id);
            return _mapper.Map<MetalRateHistory>(rateDb);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetCurrentRatesByMetalIdAsync(int metalId)
        {
            _logger.LogInformation("Getting current rates for metal {MetalId}", metalId);

            var purities = await _context.Purities
                .Where(p => p.MetalId == metalId)
                .ToListAsync();

            var rates = new List<MetalRateHistory>();

            foreach (var purity in purities)
            {
                var rate = await GetCurrentRateByPurityIdAsync(purity.Id);
                if (rate != null)
                {
                    rates.Add(rate);
                }
            }

            return rates;
        }

        public async Task<IEnumerable<MetalRateHistory>> GetAllCurrentRatesAsync()
        {
            _logger.LogInformation("Getting all current rates");

            var metals = await _context.Metals.ToListAsync();
            var allRates = new List<MetalRateHistory>();

            foreach (var metal in metals)
            {
                var metalRates = await GetCurrentRatesByMetalIdAsync(metal.Id);
                allRates.AddRange(metalRates);
            }

            return allRates;
        }

        public async Task<MetalRateHistory> AddMetalRateAsync(MetalRateHistory metalRate)
        {
            _logger.LogInformation("Adding new metal rate for purity {PurityId}: {Rate}", metalRate.PurityId, metalRate.RatePerGram);

            var metalRateDb = _mapper.Map<MetalRateHistoryDb>(metalRate);
            await _context.MetalRateHistories.AddAsync(metalRateDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Metal rate added successfully with ID {Id}", metalRateDb.Id);
            return _mapper.Map<MetalRateHistory>(metalRateDb);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetRateHistoryByPurityIdAsync(int purityId)
        {
            _logger.LogInformation("Getting rate history for purity {PurityId}", purityId);

            var ratesDb = await _context.MetalRateHistories
                .Where(g => g.PurityId == purityId)
                .OrderByDescending(g => g.EffectiveDate)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<MetalRateHistory>>(ratesDb);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetRateHistoryByMetalIdAsync(int metalId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Getting rate history for metal {MetalId} from {StartDate} to {EndDate}", metalId, startDate, endDate);

            var purityIds = await _context.Purities
                .Where(p => p.MetalId == metalId)
                .Select(p => p.Id)
                .ToListAsync();

            var ratesDb = await _context.MetalRateHistories
                .Where(g => purityIds.Contains(g.PurityId) &&
                           g.EffectiveDate >= startDate &&
                           g.EffectiveDate <= endDate)
                .OrderByDescending(g => g.EffectiveDate)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<MetalRateHistory>>(ratesDb);
        }

        public async Task<MetalRateHistory> UpdateMetalRateAsync(MetalRateHistory metalRate)
        {
            _logger.LogInformation("Updating metal rate ID {Id}: {Rate}", metalRate.Id, metalRate.RatePerGram);

            // Fetch the existing entity (this ensures proper tracking)
            var existingEntity = await _context.MetalRateHistories.FindAsync(metalRate.Id);
            if (existingEntity == null)
            {
                throw new InvalidOperationException($"Metal rate with ID {metalRate.Id} not found");
            }

            // Update properties directly on the tracked entity
            existingEntity.RatePerGram = metalRate.RatePerGram;
            existingEntity.EffectiveDate = metalRate.EffectiveDate;
            existingEntity.UpdatedBy = metalRate.UpdatedBy;
            existingEntity.UpdatedDate = metalRate.UpdatedDate;
            existingEntity.StatusId = metalRate.StatusId;

            await _context.SaveChangesAsync();

            return _mapper.Map<MetalRateHistory>(existingEntity);
        }

        public async Task<decimal> GetLatestRatePerGramAsync(int purityId)
        {
            _logger.LogInformation("Getting latest rate per gram for purity {PurityId}", purityId);

            var latestRate = await _context.MetalRateHistories
                .Where(g => g.PurityId == purityId)
                .OrderByDescending(g => g.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No rate found for purity {PurityId}, returning default rate", purityId);
                return 0;
            }

            return latestRate.RatePerGram;
        }
    }
}
