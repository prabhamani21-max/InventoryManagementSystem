using AutoMapper;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class JewelleryItemService : IJewelleryItemService
    {
        private readonly IJewelleryItemRepository _jewelleryItemRepository;
        private readonly IPurityRepository _purityRepository;
        private readonly IMetalRateService _metalRateService;
        private readonly IMapper _mapper;
        private readonly ILogger<JewelleryItemService> _logger;

        public JewelleryItemService(
            IJewelleryItemRepository jewelleryItemRepository,
            IPurityRepository purityRepository,
            IMetalRateService metalRateService,
            IMapper mapper,
            ILogger<JewelleryItemService> logger)
        {
            _jewelleryItemRepository = jewelleryItemRepository;
            _purityRepository = purityRepository;
            _metalRateService = metalRateService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<JewelleryItem> GetJewelleryItemByIdAsync(long id)
        {
            return await _jewelleryItemRepository.GetJewelleryItemByIdAsync(id);
        }

        public async Task<IEnumerable<JewelleryItem>> GetAllJewelleryItemsAsync()
        {
            return await _jewelleryItemRepository.GetAllJewelleryItemsAsync();
        }

        public async Task<JewelleryItem> CreateJewelleryItemAsync(JewelleryItem jewelleryItem)
        {
            // Validate Purity belongs to selected Metal
            var purity = await _purityRepository.GetPurityByIdAsync(jewelleryItem.PurityId);
            if (purity == null)
            {
                throw new InvalidOperationException($"Purity ID {jewelleryItem.PurityId} not found");
            }

            if (purity.MetalId != jewelleryItem.MetalId)
            {
                _logger.LogWarning(
                    "Metal-Purity mismatch: PurityId={PurityId} (MetalId={PurityMetalId}) does not match JewelleryItem.MetalId={JewelleryMetalId}",
                    jewelleryItem.PurityId,
                    purity.MetalId,
                    jewelleryItem.MetalId
                );
                throw new InvalidOperationException(
                    $"Purity ID {jewelleryItem.PurityId} (MetalId={purity.MetalId}) " +
                    $"does not match the selected MetalId={jewelleryItem.MetalId}. " +
                    $"Gold items can only use Gold purities (22K, 24K, etc)."
                );
            }

            return await _jewelleryItemRepository.CreateJewelleryItemAsync(jewelleryItem);
        }

        public async Task<JewelleryItem> UpdateJewelleryItemAsync(JewelleryItem jewelleryItem)
        {
            return await _jewelleryItemRepository.UpdateJewelleryItemAsync(jewelleryItem);
        }

        public async Task<bool> DeleteJewelleryItemAsync(long id)
        {
            return await _jewelleryItemRepository.DeleteJewelleryItemAsync(id);
        }

        /// <summary>
        /// Calculate metal amount based on current rate for a jewellery item
        /// </summary>
        public async Task<decimal> CalculateMetalAmountAsync(long jewelleryItemId)
        {
            _logger.LogInformation("Calculating metal amount for jewellery item {JewelleryItemId}", jewelleryItemId);

            var itemDb = await _jewelleryItemRepository.GetJewelleryItemDbByIdAsync(jewelleryItemId);
            if (itemDb == null)
            {
                _logger.LogWarning("Jewellery item not found: {JewelleryItemId}", jewelleryItemId);
                return 0;
            }

            // Get current metal rate based on purity
            var currentRate = await _metalRateService.GetCurrentRateByPurityIdAsync(itemDb.PurityId);
            if (currentRate == null)
            {
                _logger.LogWarning("No metal rate found for purity {PurityId}", itemDb.PurityId);
                return 0;
            }

            var amount = itemDb.NetMetalWeight * currentRate.RatePerGram;
            _logger.LogInformation("Calculated metal amount: {Amount} for item {JewelleryItemId}", amount, jewelleryItemId);

            return amount;
        }

        /// <summary>
        /// Get current metal rate for a jewellery item
        /// </summary>
        public async Task<decimal> GetCurrentMetalRateAsync(long jewelleryItemId)
        {
            var itemDb = await _jewelleryItemRepository.GetJewelleryItemDbByIdAsync(jewelleryItemId);
            if (itemDb == null)
            {
                _logger.LogWarning("Jewellery item not found: {JewelleryItemId}", jewelleryItemId);
                return 0;
            }

            var currentRate = await _metalRateService.GetCurrentRateByPurityIdAsync(itemDb.PurityId);
            return currentRate?.RatePerGram ?? 0;
        }


        /// <summary>
        /// Get jewellery item with metal details
        /// </summary>
        public async Task<JewelleryItem> GetJewelleryItemWithMetalAmountAsync(long jewelleryItemId)
        {
            _logger.LogInformation("Getting jewellery item with metal amount for {JewelleryItemId}", jewelleryItemId);

            var itemDb = await _jewelleryItemRepository.GetJewelleryItemDbByIdAsync(jewelleryItemId);
            if (itemDb == null)
            {
                _logger.LogWarning("Jewellery item not found: {JewelleryItemId}", jewelleryItemId);
                return null;
            }

            return await _jewelleryItemRepository.GetJewelleryItemByIdAsync(jewelleryItemId);
        }

        /// <summary>
        /// Get all jewellery items
        /// </summary>
        public async Task<IEnumerable<JewelleryItem>> GetAllJewelleryItemsWithMetalAmountsAsync()
        {
            _logger.LogInformation("Getting all jewellery items");
            return await _jewelleryItemRepository.GetAllJewelleryItemsAsync();
        }
    }
}
