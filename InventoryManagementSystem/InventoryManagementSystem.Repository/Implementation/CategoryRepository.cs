using AutoMapper;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(
            AppDbContext context,
            IMapper mapper,
            ILogger<CategoryRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<Category>>(categories);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return _mapper.Map<Category>(category);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            return _mapper.Map<Category>(category);
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Where(c => c.StatusId == 1)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<Category>>(categories);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
        {
            var subCategories = await _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<Category>>(subCategories);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var entity = _mapper.Map<CategoryDb>(category);
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Category {CategoryName} created with ID {CategoryId}", category.Name, entity.Id);
            
            return _mapper.Map<Category>(entity);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var entity = await _context.Categories.FindAsync(category.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.Name = category.Name;
            entity.Description = category.Description;
            entity.ParentId = category.ParentId;
            entity.StatusId = category.StatusId;
            entity.UpdatedBy = category.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CategoryId} updated", category.Id);

            return _mapper.Map<Category>(entity);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var entity = await _context.Categories.FindAsync(id);
            
            if (entity == null)
            {
                _logger.LogWarning("Category {CategoryId} not found for deletion", id);
                return false;
            }

            // Check if category has subcategories
            var hasSubCategories = await _context.Categories.AnyAsync(c => c.ParentId == id);
            if (hasSubCategories)
            {
                _logger.LogWarning("Cannot delete category {CategoryId} because it has subcategories", id);
                throw new InvalidOperationException("Cannot delete category with subcategories");
            }

            // Check if category is used by jewellery items
            var hasJewelleryItems = await _context.JewelleryItems.AnyAsync(ji => ji.CategoryId == id);
            if (hasJewelleryItems)
            {
                // Soft delete - just mark as inactive
                entity.StatusId = 3;
                entity.UpdatedDate = DateTime.UtcNow;
                _context.Categories.Update(entity);
            }
            else
            {
                // Hard delete
                _context.Categories.Remove(entity);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Category {CategoryId} deleted", id);
            
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CategoryExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}
