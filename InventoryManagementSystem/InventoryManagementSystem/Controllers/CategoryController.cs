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
            _logger = logger;
        }

        /// <summary>
        /// Get all categories (hierarchical tree)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                
                return Ok(new
                {
                    success = true,
                    data = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve categories");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving categories" });
            }
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve category {CategoryId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the category" });
            }
        }

        /// <summary>
        /// Get category by name
        /// </summary>
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve category by name {CategoryName}", name);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the category" });
            }
        }

        /// <summary>
        /// Get all active categories
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveCategoriesAsync();
                
                return Ok(new
                {
                    success = true,
                    data = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve active categories");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving categories" });
            }
        }

        /// <summary>
        /// Get subcategories by parent ID
        /// </summary>
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetSubCategories(int parentId)
        {
            try
            {
                var subCategories = await _categoryService.GetSubCategoriesAsync(parentId);
                
                return Ok(new
                {
                    success = true,
                    data = subCategories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve subcategories for parent {ParentId}", parentId);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving subcategories" });
            }
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category creation failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Category creation failed");
                return StatusCode(500, new { success = false, message = "An error occurred while creating the category" });
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto categoryDto)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category update failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Category update failed for {CategoryId}", categoryDto.Id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating the category" });
            }
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category deletion failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Category deletion failed for {CategoryId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the category" });
            }
        }

        /// <summary>
        /// Activate a category
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            try
            {
                await _categoryService.ActivateCategoryAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Category activated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category activation failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Category activation failed for {CategoryId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while activating the category" });
            }
        }

        /// <summary>
        /// Deactivate a category
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            try
            {
                await _categoryService.DeactivateCategoryAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Category deactivated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category deactivation failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Category deactivation failed for {CategoryId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while deactivating the category" });
            }
        }
    }
}
