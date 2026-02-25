using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class ItemStockRepository : IItemStockRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ItemStockRepository> _logger;

        public ItemStockRepository(AppDbContext context, IMapper mapper, ILogger<ItemStockRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ItemStock> GetItemStockByIdAsync(int id)
        {
            var itemStockDb = await _context.ItemStocks
                .Include(i => i.JewelleryItem)
                .Include(i => i.Warehouse)
                .Include(i => i.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
            return _mapper.Map<ItemStock>(itemStockDb);
        }

        public async Task<ItemStock> GetItemStockByJewelleryItemIdAsync(long jewelleryItemId, int? warehouseId = null)
        {
            var query = _context.ItemStocks
                .Include(i => i.JewelleryItem)
                .Include(i => i.Warehouse)
                .Include(i => i.Status)
                .Where(i => i.JewelleryItemId == jewelleryItemId);

            if (warehouseId.HasValue)
            {
                query = query.Where(i => i.WarehouseId == warehouseId.Value);
            }

            var itemStockDb = await query.FirstOrDefaultAsync();
            return _mapper.Map<ItemStock>(itemStockDb);
        }

        public async Task<IEnumerable<ItemStock>> GetAllItemStocksAsync()
        {
            var itemStocksDb = await _context.ItemStocks
                .Include(i => i.JewelleryItem)
                .Include(i => i.Warehouse)
                .Include(i => i.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<ItemStock>>(itemStocksDb);
        }

        public async Task<ItemStock> CreateItemStockAsync(ItemStock itemStock)
        {
            var entity = _mapper.Map<ItemStockDb>(itemStock);
            await _context.ItemStocks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ItemStock>(entity);
        }

        public async Task<ItemStock> UpdateItemStockAsync(ItemStock itemStock)
        {
            var entity = await _context.ItemStocks.FindAsync(itemStock.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.JewelleryItemId = itemStock.JewelleryItemId;
            entity.WarehouseId = itemStock.WarehouseId;
            entity.Quantity = itemStock.Quantity;
            entity.ReservedQuantity = itemStock.ReservedQuantity;
            entity.StatusId = itemStock.StatusId;
            entity.UpdatedBy = itemStock.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<ItemStock>(entity);
        }

        public async Task<bool> DeleteItemStockAsync(int id)
        {
            var entity = await _context.ItemStocks.FindAsync(id);
            if (entity == null) return false;
            _context.ItemStocks.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // ==================== STOCK VALIDATION AND MANAGEMENT ====================

        /// <summary>
        /// Checks if sufficient stock is available for the requested quantity.
        /// Available quantity = Total Quantity - Reserved Quantity
        /// </summary>
        public async Task<bool> CheckStockAvailabilityAsync(long jewelleryItemId, int requestedQuantity, int? warehouseId = null)
        {
            var stock = await GetItemStockByJewelleryItemIdAsync(jewelleryItemId, warehouseId);
            if (stock == null)
            {
                _logger.LogWarning("No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                return false;
            }

            var availableQuantity = stock.Quantity - stock.ReservedQuantity;
            return availableQuantity >= requestedQuantity;
        }

        /// <summary>
        /// Validates stock availability for multiple items in an order
        /// </summary>
        public async Task<StockValidationResult> ValidateStockForOrderAsync(IEnumerable<StockValidationRequest> items)
        {
            var result = new StockValidationResult { IsValid = true };

            foreach (var item in items)
            {
                var stock = await _context.ItemStocks
                    .Include(s => s.JewelleryItem)
                    .FirstOrDefaultAsync(s => s.JewelleryItemId == item.JewelleryItemId &&
                        (!item.WarehouseId.HasValue || s.WarehouseId == item.WarehouseId.Value));

                if (stock == null)
                {
                    result.IsValid = false;
                    result.Errors.Add(new StockValidationError
                    {
                        JewelleryItemId = item.JewelleryItemId,
                        ItemName = "Unknown Item",
                        RequestedQuantity = item.RequestedQuantity,
                        AvailableQuantity = 0,
                        Message = $"No stock record found for item ID {item.JewelleryItemId}"
                    });
                    continue;
                }

                var availableQuantity = stock.Quantity - stock.ReservedQuantity;
                if (availableQuantity < item.RequestedQuantity)
                {
                    result.IsValid = false;
                    result.Errors.Add(new StockValidationError
                    {
                        JewelleryItemId = item.JewelleryItemId,
                        ItemName = stock.JewelleryItem?.Name ?? "Unknown",
                        RequestedQuantity = item.RequestedQuantity,
                        AvailableQuantity = availableQuantity,
                        Message = $"Insufficient stock for '{stock.JewelleryItem?.Name}'. Requested: {item.RequestedQuantity}, Available: {availableQuantity}"
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Reserves stock for an order. Increases ReservedQuantity.
        /// </summary>
        public async Task<bool> ReserveStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var query = _context.ItemStocks.Where(s => s.JewelleryItemId == jewelleryItemId);
                if (warehouseId.HasValue)
                {
                    query = query.Where(s => s.WarehouseId == warehouseId.Value);
                }

                var stock = await query.FirstOrDefaultAsync();
                if (stock == null)
                {
                    _logger.LogWarning("Cannot reserve stock: No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                    return false;
                }

                var availableQuantity = stock.Quantity - stock.ReservedQuantity;
                if (availableQuantity < quantity)
                {
                    _logger.LogWarning("Cannot reserve stock: Insufficient available quantity. Requested: {Requested}, Available: {Available}",
                        quantity, availableQuantity);
                    return false;
                }

                stock.ReservedQuantity += quantity;
                stock.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Reserved {Quantity} units of JewelleryItemId: {JewelleryItemId}", quantity, jewelleryItemId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error reserving stock for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                throw;
            }
        }

        /// <summary>
        /// Releases reserved stock (e.g., when order is cancelled before invoice).
        /// Decreases ReservedQuantity.
        /// </summary>
        public async Task<bool> ReleaseReservedStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var query = _context.ItemStocks.Where(s => s.JewelleryItemId == jewelleryItemId);
                if (warehouseId.HasValue)
                {
                    query = query.Where(s => s.WarehouseId == warehouseId.Value);
                }

                var stock = await query.FirstOrDefaultAsync();
                if (stock == null)
                {
                    _logger.LogWarning("Cannot release reserved stock: No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                    return false;
                }

                if (stock.ReservedQuantity < quantity)
                {
                    _logger.LogWarning("Cannot release reserved stock: Reserved quantity is less than requested release. Reserved: {Reserved}, Requested: {Requested}",
                        stock.ReservedQuantity, quantity);
                    // Release what's available
                    quantity = stock.ReservedQuantity;
                }

                stock.ReservedQuantity -= quantity;
                stock.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Released {Quantity} units of reserved stock for JewelleryItemId: {JewelleryItemId}", quantity, jewelleryItemId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error releasing reserved stock for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                throw;
            }
        }

        /// <summary>
        /// Deducts stock when invoice is generated.
        /// Decreases both Quantity and ReservedQuantity.
        /// </summary>
        public async Task<bool> DeductStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var query = _context.ItemStocks.Where(s => s.JewelleryItemId == jewelleryItemId);
                if (warehouseId.HasValue)
                {
                    query = query.Where(s => s.WarehouseId == warehouseId.Value);
                }

                var stock = await query.FirstOrDefaultAsync();
                if (stock == null)
                {
                    _logger.LogWarning("Cannot deduct stock: No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                    return false;
                }

                if (stock.Quantity < quantity)
                {
                    _logger.LogWarning("Cannot deduct stock: Insufficient quantity. Available: {Available}, Requested: {Requested}",
                        stock.Quantity, quantity);
                    return false;
                }

                if (stock.ReservedQuantity < quantity)
                {
                    _logger.LogWarning("Reserved quantity is less than deduction amount. Deducting from main quantity anyway. Reserved: {Reserved}, Requested: {Requested}",
                        stock.ReservedQuantity, quantity);
                }

                stock.Quantity -= quantity;
                // Also reduce reserved quantity (the stock was reserved before invoice)
                stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - quantity);
                stock.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Deducted {Quantity} units from stock for JewelleryItemId: {JewelleryItemId}", quantity, jewelleryItemId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deducting stock for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                throw;
            }
        }

        /// <summary>
        /// Restores stock (e.g., when invoice is cancelled).
        /// Increases Quantity.
        /// </summary>
        public async Task<bool> RestoreStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var query = _context.ItemStocks.Where(s => s.JewelleryItemId == jewelleryItemId);
                if (warehouseId.HasValue)
                {
                    query = query.Where(s => s.WarehouseId == warehouseId.Value);
                }

                var stock = await query.FirstOrDefaultAsync();
                if (stock == null)
                {
                    _logger.LogWarning("Cannot restore stock: No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                    return false;
                }

                stock.Quantity += quantity;
                stock.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Restored {Quantity} units to stock for JewelleryItemId: {JewelleryItemId}", quantity, jewelleryItemId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error restoring stock for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                throw;
            }
        }
    }
}