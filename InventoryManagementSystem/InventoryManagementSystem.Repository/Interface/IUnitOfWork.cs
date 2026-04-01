using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManagementSystem.Repository.Interface
{
    /// <summary>
    /// Unit of Work Interface
    /// Provides transaction management and coordinates repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Commits all pending changes
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}