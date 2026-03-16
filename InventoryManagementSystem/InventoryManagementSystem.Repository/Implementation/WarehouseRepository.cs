using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WarehouseRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int id)
        {
            var warehouseDb = await _context.Warehouses
                .Include(w => w.Manager)
                .Include(w => w.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
            return _mapper.Map<Warehouse>(warehouseDb);
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            var warehousesDb = await _context.Warehouses
                .Include(w => w.Manager)
                .Include(w => w.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Warehouse>>(warehousesDb);
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            var entity = _mapper.Map<WarehouseDb>(warehouse);
            await _context.Warehouses.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Warehouse>(entity);
        }

        public async Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse)
        {
            var entity = await _context.Warehouses.FindAsync(warehouse.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.Name = warehouse.Name;
            entity.Address = warehouse.Address;
            entity.ManagerId = warehouse.ManagerId;
            entity.StatusId = warehouse.StatusId;
            entity.UpdatedBy = warehouse.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<Warehouse>(entity);
        }

        public async Task<bool> DeleteWarehouseAsync(int id)
        {
            var entity = await _context.Warehouses.FindAsync(id);
            if (entity == null) return false;
            _context.Warehouses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}