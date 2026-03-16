using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SupplierRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            var supplierDb = await _context.Suppliers
                .Include(s => s.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return _mapper.Map<Supplier>(supplierDb);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            var suppliersDb = await _context.Suppliers
                .Include(s => s.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Supplier>>(suppliersDb);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            var entity = _mapper.Map<SupplierDb>(supplier);
            await _context.Suppliers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Supplier>(entity);
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            var existingEntity = await _context.Suppliers.FindAsync(supplier.Id);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {supplier.Id} not found.");
            }

            // Manual mapping - preserve CreatedBy and CreatedDate
            existingEntity.Name = supplier.Name;
            existingEntity.ContactPerson = supplier.ContactPerson;
            existingEntity.Email = supplier.Email;
            existingEntity.Phone = supplier.Phone;
            existingEntity.Address = supplier.Address;
            existingEntity.GSTNumber = supplier.GSTNumber;
            existingEntity.TANNumber = supplier.TANNumber;
            existingEntity.StatusId = supplier.StatusId;
            existingEntity.UpdatedBy = supplier.UpdatedBy;
            existingEntity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<Supplier>(existingEntity);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var entity = await _context.Suppliers.FindAsync(id);
            if (entity == null) return false;
            _context.Suppliers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GSTNumberExistsAsync(string gstNumber, int? excludeId = null)
        {
            return await _context.Suppliers
                .AnyAsync(s => s.GSTNumber == gstNumber && (excludeId == null || s.Id != excludeId));
        }

        public async Task<bool> TANNumberExistsAsync(string tanNumber, int? excludeId = null)
        {
            return await _context.Suppliers
                .AnyAsync(s => s.TANNumber == tanNumber && (excludeId == null || s.Id != excludeId));
        }
    }
}