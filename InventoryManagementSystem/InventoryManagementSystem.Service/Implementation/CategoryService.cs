using AutoMapper;
using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return BuildCategoryTree(categories.ToList());
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                return null;
            }

            var response = _mapper.Map<CategoryResponseDto>(category);
            
            // Get subcategories
            var subCategories = await _categoryRepository.GetSubCategoriesAsync(id);
            response.SubCategories = BuildCategoryTree(subCategories.ToList());
            
            return response;
        }

        public async Task<CategoryResponseDto?> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryRepository.GetCategoryByNameAsync(name);
            
            if (category == null)
            {
                return null;
            }

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetActiveCategoriesAsync()
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync();
            return BuildCategoryTree(categories.ToList());
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetSubCategoriesAsync(int parentId)
        {
            var subCategories = await _categoryRepository.GetSubCategoriesAsync(parentId);
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(subCategories);
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            // Validate parent category exists
            if (categoryDto.ParentId.HasValue)
            {
                var parentExists = await _categoryRepository.CategoryExistsAsync(categoryDto.ParentId.Value);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"Parent category with ID {categoryDto.ParentId} does not exist");
                }
            }

            // Check for duplicate name
            var exists = await _categoryRepository.CategoryExistsByNameAsync(categoryDto.Name);
            if (exists)
            {
                throw new InvalidOperationException($"Category with name '{categoryDto.Name}' already exists");
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedDate = DateTime.UtcNow;
            category.CreatedBy = 1; // TODO: Get from current user

            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
            
            _logger.LogInformation("Category {CategoryName} created successfully", categoryDto.Name);
            
            return _mapper.Map<CategoryResponseDto>(createdCategory);
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(CategoryUpdateDto categoryDto)
        {
            // Validate category exists
            var exists = await _categoryRepository.CategoryExistsAsync(categoryDto.Id);
            if (!exists)
            {
                throw new InvalidOperationException($"Category with ID {categoryDto.Id} does not exist");
            }

            // Check for duplicate name (excluding current category)
            var nameExists = await _categoryRepository.CategoryExistsByNameAsync(categoryDto.Name, categoryDto.Id);
            if (nameExists)
            {
                throw new InvalidOperationException($"Category with name '{categoryDto.Name}' already exists");
            }

            // Validate parent category
            if (categoryDto.ParentId.HasValue && categoryDto.ParentId.Value != categoryDto.Id)
            {
                var parentExists = await _categoryRepository.CategoryExistsAsync(categoryDto.ParentId.Value);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"Parent category with ID {categoryDto.ParentId} does not exist");
                }
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.UpdatedDate = DateTime.UtcNow;
            category.UpdatedBy = 1; // TODO: Get from current user

            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);
            
            _logger.LogInformation("Category {CategoryId} updated successfully", categoryDto.Id);
            
            return _mapper.Map<CategoryResponseDto>(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                return await _categoryRepository.DeleteCategoryAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete category {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> ActivateCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {id} does not exist");
            }

            category.StatusId = 1;
            category.UpdatedDate = DateTime.UtcNow;
            
            await _categoryRepository.UpdateCategoryAsync(category);
            
            _logger.LogInformation("Category {CategoryId} activated", id);
            
            return true;
        }

        public async Task<bool> DeactivateCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {id} does not exist");
            }

            category.StatusId = 2;
            category.UpdatedDate = DateTime.UtcNow;
            
            await _categoryRepository.UpdateCategoryAsync(category);
            
            _logger.LogInformation("Category {CategoryId} deactivated", id);
            
            return true;
        }

        /// <summary>
        /// Build hierarchical tree structure from flat category list
        /// </summary>
        private List<CategoryResponseDto> BuildCategoryTree(List<Category> categories)
        {
            // First, map all categories to DTOs
            var categoryDtoDict = new Dictionary<int, CategoryResponseDto>();
            
            foreach (var category in categories)
            {
                var dto = _mapper.Map<CategoryResponseDto>(category);
                dto.SubCategories = new List<CategoryResponseDto>();
                categoryDtoDict[dto.Id] = dto;
            }

            // Build the tree
            var rootCategories = new List<CategoryResponseDto>();
            
            foreach (var category in categories)
            {
                if (categoryDtoDict.TryGetValue(category.Id, out var dto))
                {
                    if (category.ParentId.HasValue && categoryDtoDict.ContainsKey(category.ParentId.Value))
                    {
                        var parent = categoryDtoDict[category.ParentId.Value];
                        parent.SubCategories!.Add(dto);
                    }
                    else
                    {
                        rootCategories.Add(dto);
                    }
                }
            }

            return rootCategories;
        }
    }
}
