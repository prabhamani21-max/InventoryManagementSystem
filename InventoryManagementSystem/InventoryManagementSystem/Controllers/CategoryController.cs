using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    /// <summary>
    /// Category API Controller
    /// Manages jewellery categories (Rings, Necklaces, Earrings, etc.)
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories (hierarchical tree)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            return Ok(new
            {
                success = true,
                data = categories
            });
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }

            return Ok(new
            {
                success = true,
                data = category
            });
        }

        /// <summary>
        /// Get category by name
        /// </summary>
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _categoryService.GetCategoryByNameAsync(name);

            if (category == null)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }

            return Ok(new
            {
                success = true,
                data = category
            });
        }

        /// <summary>
        /// Get all active categories
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();

            return Ok(new
            {
                success = true,
                data = categories
            });
        }

        /// <summary>
        /// Get subcategories by parent ID
        /// </summary>
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetSubCategories(int parentId)
        {
            var subCategories = await _categoryService.GetSubCategoriesAsync(parentId);

            return Ok(new
            {
                success = true,
                data = subCategories
            });
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid category data", errors = ModelState });
            }

            var category = await _categoryService.CreateCategoryAsync(categoryDto);

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, new
            {
                success = true,
                message = "Category created successfully",
                data = category
            });
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid category data", errors = ModelState });
            }

            var category = await _categoryService.UpdateCategoryAsync(categoryDto);

            return Ok(new
            {
                success = true,
                message = "Category updated successfully",
                data = category
            });
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }

            return Ok(new
            {
                success = true,
                message = "Category deleted successfully"
            });
        }

        /// <summary>
        /// Activate a category
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            await _categoryService.ActivateCategoryAsync(id);

            return Ok(new
            {
                success = true,
                message = "Category activated successfully"
            });
        }

        /// <summary>
        /// Deactivate a category
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            await _categoryService.DeactivateCategoryAsync(id);

            return Ok(new
            {
                success = true,
                message = "Category deactivated successfully"
            });
        }
    }
}
