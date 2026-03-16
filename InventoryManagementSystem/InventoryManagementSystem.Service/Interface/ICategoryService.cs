using InventoryManagementSystem.Common.Models;
using InventoryManagementSytem.Common.Dtos;

namespace InventoryManagementSystem.Service.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto?> GetCategoryByNameAsync(string name);
        Task<IEnumerable<CategoryResponseDto>> GetActiveCategoriesAsync();
        Task<IEnumerable<CategoryResponseDto>> GetSubCategoriesAsync(int parentId);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryResponseDto> UpdateCategoryAsync(CategoryUpdateDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> ActivateCategoryAsync(int id);
        Task<bool> DeactivateCategoryAsync(int id);
    }
}
