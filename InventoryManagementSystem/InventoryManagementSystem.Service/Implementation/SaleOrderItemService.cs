using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSytem.Common.Enums;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class SaleOrderItemService : ISaleOrderItemService
    {
        private readonly ISaleOrderItemRepository _saleOrderItemRepository;
        private readonly IJewelleryItemRepository _jewelleryItemRepository;
        private readonly IItemStoneRepository _itemStoneRepository;
        private readonly IItemStockRepository _itemStockRepository;
        private readonly IMetalRateService _metalRateService;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<SaleOrderItemService> _logger;

        public SaleOrderItemService(
            ISaleOrderItemRepository saleOrderItemRepository,
            IJewelleryItemRepository jewelleryItemRepository,
            IItemStoneRepository itemStoneRepository,
            IItemStockRepository itemStockRepository,
            IMetalRateService metalRateService,
            ICurrentUser currentUser,
            IMapper mapper,
            ILogger<SaleOrderItemService> logger)
        {
            _saleOrderItemRepository = saleOrderItemRepository;
            _jewelleryItemRepository = jewelleryItemRepository;
            _itemStoneRepository = itemStoneRepository;
            _itemStockRepository = itemStockRepository;
            _metalRateService = metalRateService;
            _currentUser = currentUser;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SaleOrderItem> GetSaleOrderItemByIdAsync(long id)
        {
            return await _saleOrderItemRepository.GetSaleOrderItemByIdAsync(id);
        }

        public async Task<IEnumerable<SaleOrderItem>> GetAllSaleOrderItemsAsync()
        {
            return await _saleOrderItemRepository.GetAllSaleOrderItemsAsync();
        }

        public async Task<IEnumerable<SaleOrderItem>> GetSaleOrderItemsBySaleOrderIdAsync(long saleOrderId)
        {
            return await _saleOrderItemRepository.GetSaleOrderItemsBySaleOrderIdAsync(saleOrderId);
        }

        /// <summary>
        /// Creates a sale order item with automatic price calculation.
        /// Calculates: MetalAmount, MakingCharges, WastageAmount, ItemSubtotal, TaxableAmount, GstAmount, TotalAmount
        /// </summary>
        public async Task<SaleOrderItem> CreateSaleOrderItemWithCalculationAsync(
            long saleOrderId,
            long jewelleryItemId,
            decimal? discountAmount,
            decimal gstPercentage,
            decimal? stoneAmount,
            int quantity)
        {
            // 1. Fetch jewellery item details
            var jewelleryItem = await _jewelleryItemRepository.GetJewelleryItemDbByIdAsync(jewelleryItemId)
                ?? throw new InvalidOperationException($"Jewellery item with ID {jewelleryItemId} not found");

            // 2. Validate stock availability before proceeding
            var stockAvailable = await _itemStockRepository.CheckStockAvailabilityAsync(jewelleryItemId, quantity);
            if (!stockAvailable)
            {
                var stock = await _itemStockRepository.GetItemStockByJewelleryItemIdAsync(jewelleryItemId);
                var availableQty = stock != null ? stock.Quantity - stock.ReservedQuantity : 0;
                throw new InvalidOperationException(
                    $"Insufficient stock for '{jewelleryItem.Name}'. Requested: {quantity}, Available: {availableQty}");
            }

            // 3. Fetch latest metal rate for the item's purity
            _logger.LogInformation(
                "Metal Rate Lookup: MetalId={MetalId}, PurityId={PurityId}, NetMetalWeight={NetMetalWeight}",
                jewelleryItem.MetalId,
                jewelleryItem.PurityId,
                jewelleryItem.NetMetalWeight
            );

            // Get the full rate object to validate the effective date
            var metalRateInfo = await _metalRateService.GetCurrentRateByPurityIdAsync(jewelleryItem.PurityId);
            
            if (metalRateInfo == null)
            {
                _logger.LogWarning(
                    "No metal rate found for MetalId={MetalId}, PurityId={PurityId}. Please add a rate for this purity.",
                    jewelleryItem.MetalId,
                    jewelleryItem.PurityId
                );
                throw new InvalidOperationException($"No metal rate found for purity ID {jewelleryItem.PurityId}. Please add a rate before creating the sale order.");
            }

            // Validate that the rate is current (today's rate)
            var today = DateTime.UtcNow.Date;
            var rateDate = metalRateInfo.EffectiveDate.Date;
            
            if (rateDate < today)
            {
                _logger.LogWarning(
                    "Stale metal rate detected for PurityId={PurityId}. Rate date: {RateDate}, Current date: {CurrentDate}",
                    jewelleryItem.PurityId,
                    rateDate.ToString("yyyy-MM-dd"),
                    today.ToString("yyyy-MM-dd")
                );
                throw new InvalidOperationException(
                    $"Metal rate is outdated (last updated: {rateDate:yyyy-MM-dd}). " +
                    "Please update today's metal rate before creating the sale order.");
            }

            var metalRate = metalRateInfo.RatePerGram;
            
            _logger.LogInformation(
                "Metal Rate Result: PurityId={PurityId}, RatePerGram={RatePerGram}, EffectiveDate={EffectiveDate}, MetalAmount={MetalAmount}",
                jewelleryItem.PurityId,
                metalRate,
                metalRateInfo.EffectiveDate.ToString("yyyy-MM-dd"),
                jewelleryItem.NetMetalWeight * metalRate
            );

            if (metalRate <= 0)
            {
                _logger.LogWarning(
                    "Invalid metal rate for MetalId={MetalId}, PurityId={PurityId}. Rate must be greater than zero.",
                    jewelleryItem.MetalId,
                    jewelleryItem.PurityId
                );
                throw new InvalidOperationException($"Invalid metal rate for purity ID {jewelleryItem.PurityId}. Rate must be greater than zero.");
            }

            // 4. Fetch stone amount if applicable
            decimal calculatedStoneAmount = 0;
            if (stoneAmount.HasValue)
            {
                calculatedStoneAmount = stoneAmount.Value;
            }
          

            // 5. Calculate all pricing components
            var calculation = CalculatePricing(
                jewelleryItem: jewelleryItem,
                metalRatePerGram: metalRate,
                gstPercentage: gstPercentage,
                discountAmount: discountAmount ?? 0m,
                stoneAmount: calculatedStoneAmount,
                quantity: quantity
            );

            // 6. Create the SaleOrderItem model
            var saleOrderItem = new SaleOrderItem
            {
                SaleOrderId = saleOrderId,
                JewelleryItemId = jewelleryItemId,
                ItemCode = jewelleryItem.ItemCode,
                ItemName = jewelleryItem.Name,
                Description = jewelleryItem.Description,
                Quantity = quantity,

                // Metal snapshot
                MetalId = jewelleryItem.MetalId,
                PurityId = jewelleryItem.PurityId,
                GrossWeight = jewelleryItem.GrossWeight,
                NetMetalWeight = jewelleryItem.NetMetalWeight,
                MetalRatePerGram = metalRate,
                MetalAmount = calculation.MetalAmount,

                // Making charges
                MakingChargeType = jewelleryItem.MakingChargeType,
                MakingChargeValue = jewelleryItem.MakingChargeValue,
                TotalMakingCharges = calculation.TotalMakingCharges,
                WastagePercentage = jewelleryItem.WastagePercentage,
                WastageWeight = calculation.WastageWeight,
                WastageAmount = calculation.WastageAmount,

                // Stone summary
                HasStone = jewelleryItem.HasStone,
                StoneAmount = calculatedStoneAmount,

                // Price & Tax
                ItemSubtotal = calculation.ItemSubtotal,
                DiscountAmount = discountAmount ?? 0m,
                TaxableAmount = calculation.TaxableAmount,
                GstPercentage = gstPercentage,
                GstAmount = calculation.GstAmount,
                MakingChargesGstPercentage = 5m, // 5% GST on making charges
                MakingChargesGstAmount = calculation.MakingChargesGstAmount,
                TotalGstAmount = calculation.TotalGstAmount,
                TotalAmount = calculation.TotalAmount,

                // Audit
                CreatedDate = DateTime.UtcNow,
                CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin,
                StatusId = (int)StatusEnum.Active // Active status
            };

            // 7. Create the sale order item
            var createdItem = await _saleOrderItemRepository.CreateSaleOrderItemAsync(saleOrderItem);

            // 8. Reserve stock for this item
            var reserved = await _itemStockRepository.ReserveStockAsync(jewelleryItemId, quantity);
            if (!reserved)
            {
                _logger.LogWarning("Failed to reserve stock for JewelleryItemId: {JewelleryItemId}, Quantity: {Quantity}", jewelleryItemId, quantity);
                // Note: Item is still created, but stock reservation failed - this can be handled by a background process
            }
            else
            {
                _logger.LogInformation("Successfully reserved {Quantity} units for JewelleryItemId: {JewelleryItemId}", quantity, jewelleryItemId);
            }

            return createdItem;
        }

        /// <summary>
        /// Creates a sale order item directly (for backward compatibility)
        /// </summary>
        public async Task<SaleOrderItem> CreateSaleOrderItemAsync(SaleOrderItem saleOrderItem)
        {
            return await _saleOrderItemRepository.CreateSaleOrderItemAsync(saleOrderItem);
        }

        public async Task<SaleOrderItem> UpdateSaleOrderItemAsync(SaleOrderItem saleOrderItem)
        {
            return await _saleOrderItemRepository.UpdateSaleOrderItemAsync(saleOrderItem);
        }

        /// <summary>
        /// Deletes a sale order item and releases the reserved stock
        /// </summary>
        public async Task<bool> DeleteSaleOrderItemAsync(long id)
        {
            // Get the item to find the jewellery item and quantity
            var item = await _saleOrderItemRepository.GetSaleOrderItemByIdAsync(id);
            if (item == null)
            {
                return false;
            }
            
            // Delete the item
            var deleted = await _saleOrderItemRepository.DeleteSaleOrderItemAsync(id);
            if (deleted)
            {
                // Release the reserved stock
                var released = await _itemStockRepository.ReleaseReservedStockAsync(item.JewelleryItemId, item.Quantity);
                if (released)
                {
                    _logger.LogInformation("Released {Quantity} reserved stock for JewelleryItemId: {JewelleryItemId}", 
                        item.Quantity, item.JewelleryItemId);
                }
                else
                {
                    _logger.LogWarning("Failed to release reserved stock for JewelleryItemId: {JewelleryItemId}", 
                        item.JewelleryItemId);
                }
            }
            return deleted;
        }

        /// <summary>
        /// Calculates pricing for a sale order item based on the industry-standard formula:
        /// MetalAmount + MakingCharges + WastageAmount + StoneAmount = ItemSubtotal
        /// ItemSubtotal - Discount = TaxableAmount
        /// GST Calculation:
        /// - 3% GST on (MetalAmount + WastageAmount + StoneAmount) - Metal GST
        /// - 5% GST on MakingCharges - Making Charges GST
        /// TotalAmount = TaxableAmount + Metal GST + Making Charges GST
        /// </summary>
        private PricingCalculation CalculatePricing(
            JewelleryItemDb jewelleryItem,
            decimal metalRatePerGram,
            decimal gstPercentage,
            decimal discountAmount,
            decimal stoneAmount,
            int quantity)
        {
            // Metal Amount = NetMetalWeight * MetalRatePerGram
            var metalAmount = jewelleryItem.NetMetalWeight * metalRatePerGram;

            // Making Charges calculation based on type
            decimal totalMakingCharges;
            switch (jewelleryItem.MakingChargeType)
            {
                case MakingChargeType.Percentage:
                    totalMakingCharges = (metalAmount * jewelleryItem.MakingChargeValue) / 100;
                    break;
                case MakingChargeType.Fixed:
                    totalMakingCharges = jewelleryItem.MakingChargeValue;
                    break;
                case MakingChargeType.PerGram:
                default:
                    totalMakingCharges = jewelleryItem.NetMetalWeight * jewelleryItem.MakingChargeValue;
                    break;
            }

            // Wastage calculation: WastageWeight = (NetMetalWeight * WastagePercentage) / 100
            var wastageWeight = (jewelleryItem.NetMetalWeight * jewelleryItem.WastagePercentage) / 100;
            var wastageAmount = wastageWeight * metalRatePerGram;

            // Item Subtotal = MetalAmount + MakingCharges + WastageAmount + StoneAmount
            var itemSubtotal = metalAmount + totalMakingCharges + wastageAmount + stoneAmount;

            // Taxable Amount = ItemSubtotal - Discount
            var taxableAmount = itemSubtotal - discountAmount;
            if (taxableAmount < 0) taxableAmount = 0;

            // GST Calculation - Separate GST for metal and making charges
            // Metal GST (3%) = (MetalAmount + WastageAmount + StoneAmount) * 3%
            var metalTaxableAmount = metalAmount + wastageAmount + stoneAmount;
            var metalGstAmount = (metalTaxableAmount * gstPercentage) / 100;

            // Making Charges GST (5%) = MakingCharges * 5%
            const decimal MakingChargesGstPercentage = 5m;
            var makingChargesGstAmount = (totalMakingCharges * MakingChargesGstPercentage) / 100;

            // Total GST = Metal GST + Making Charges GST
            var totalGstAmount = metalGstAmount + makingChargesGstAmount;

            // Total Amount = TaxableAmount + Total GST
            var totalAmount = taxableAmount + totalGstAmount;

            // Apply quantity multiplier
            return new PricingCalculation
            {
                MetalAmount = metalAmount * quantity,
                TotalMakingCharges = totalMakingCharges * quantity,
                WastageWeight = wastageWeight * quantity,
                WastageAmount = wastageAmount * quantity,
                ItemSubtotal = itemSubtotal * quantity,
                TaxableAmount = taxableAmount * quantity,
                GstAmount = metalGstAmount * quantity,
                MakingChargesGstAmount = makingChargesGstAmount * quantity,
                TotalGstAmount = totalGstAmount * quantity,
                TotalAmount = totalAmount * quantity
            };
        }

        /// <summary>
        /// Internal class to hold pricing calculation results
        /// </summary>
        private class PricingCalculation
        {
            public decimal MetalAmount { get; set; }
            public decimal TotalMakingCharges { get; set; }
            public decimal WastageWeight { get; set; }
            public decimal WastageAmount { get; set; }
            public decimal ItemSubtotal { get; set; }
            public decimal TaxableAmount { get; set; }
            public decimal GstAmount { get; set; } // Metal GST (3%)
            public decimal MakingChargesGstAmount { get; set; } // Making Charges GST (5%)
            public decimal TotalGstAmount { get; set; } // Total GST (Metal + Making Charges)
            public decimal TotalAmount { get; set; }
        }
    }
}
