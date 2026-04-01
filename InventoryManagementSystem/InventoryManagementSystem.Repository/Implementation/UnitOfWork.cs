using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    /// <summary>
    /// Unit of Work Implementation
    /// Provides transaction management using existing DbContext
    /// Repositories remain independently registered in DI
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Beginning new database transaction");
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Saving changes to database");
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}