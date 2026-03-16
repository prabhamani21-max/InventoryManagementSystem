using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.DTO;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public SupplierController(ISupplierService supplierService, IMapper mapper, ILogger<SupplierController> logger, ICurrentUser currentUser)
        {
            _supplierService = supplierService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            _logger.LogInformation("Fetching all suppliers");
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
            return Ok(supplierDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            _logger.LogInformation("Fetching supplier by ID: {Id}", id);
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier not found for ID: {Id}", id);
                return NotFound("Supplier not found");
            }
            var supplierDto = _mapper.Map<SupplierDto>(supplier);
            return Ok(supplierDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating supplier: {Name}", dto.Name);

            // Check GST Number uniqueness
            if (await _supplierService.GSTNumberExistsAsync(dto.GSTNumber))
            {
                ModelState.AddModelError("GSTNumber", "GST number already exists");
                return Conflict(ModelState);
            }

            // Check TAN Number uniqueness
            if (await _supplierService.TANNumberExistsAsync(dto.TANNumber))
            {
                ModelState.AddModelError("TANNumber", "TAN number already exists");
                return Conflict(ModelState);
            }

            var supplier = _mapper.Map<Supplier>(dto);
            supplier.CreatedDate = DateTime.UtcNow;
            supplier.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            supplier.StatusId = (int)StatusEnum.Active;

            var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
            var createdDto = _mapper.Map<SupplierDto>(createdSupplier);

            _logger.LogInformation("Supplier created successfully ID: {Id}", createdDto.Id);
            return Ok(createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating supplier ID: {Id}", id);

            var existingSupplier = await _supplierService.GetSupplierByIdAsync(id);
            if (existingSupplier == null)
            {
                _logger.LogWarning("Supplier not found for update ID: {Id}", id);
                return NotFound("Supplier not found");
            }

            // Check GST Number uniqueness excluding current record
            if (await _supplierService.GSTNumberExistsAsync(dto.GSTNumber, id))
            {
                ModelState.AddModelError("GSTNumber", "GST number already exists");
                return Conflict(ModelState);
            }

            // Check TAN Number uniqueness excluding current record
            if (await _supplierService.TANNumberExistsAsync(dto.TANNumber, id))
            {
                ModelState.AddModelError("TANNumber", "TAN number already exists");
                return Conflict(ModelState);
            }

            var supplier = _mapper.Map<Supplier>(dto);
            supplier.Id = id;
            supplier.UpdatedDate = DateTime.UtcNow;
            supplier.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedSupplier = await _supplierService.UpdateSupplierAsync(supplier);
            var updatedDto = _mapper.Map<SupplierDto>(updatedSupplier);

            _logger.LogInformation("Supplier updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            _logger.LogInformation("Deleting supplier ID: {Id}", id);

            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
            {
                _logger.LogWarning("Supplier not found for deletion ID: {Id}", id);
                return NotFound("Supplier not found");
            }

            _logger.LogInformation("Supplier deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}