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
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public WarehouseController(IWarehouseService warehouseService, IMapper mapper, ILogger<WarehouseController> logger, ICurrentUser currentUser)
        {
            _warehouseService = warehouseService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWarehouses()
        {
            _logger.LogInformation("Fetching all warehouses");
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            var warehouseDtos = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
            return Ok(warehouseDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouseById(int id)
        {
            _logger.LogInformation("Fetching warehouse by ID: {Id}", id);
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse not found for ID: {Id}", id);
                return NotFound("Warehouse not found");
            }
            var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
            return Ok(warehouseDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] WarehouseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating warehouse: {Name}", dto.Name);

            var warehouse = _mapper.Map<Warehouse>(dto);
            warehouse.CreatedDate = DateTime.UtcNow;
            warehouse.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            warehouse.StatusId = (int)StatusEnum.Active;

            var createdWarehouse = await _warehouseService.CreateWarehouseAsync(warehouse);
            var createdDto = _mapper.Map<WarehouseDto>(createdWarehouse);

            _logger.LogInformation("Warehouse created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetWarehouseById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating warehouse ID: {Id}", id);

            var existingWarehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (existingWarehouse == null)
            {
                _logger.LogWarning("Warehouse not found for update ID: {Id}", id);
                return NotFound("Warehouse not found");
            }

            var warehouse = _mapper.Map<Warehouse>(dto);
            warehouse.Id = id;
            warehouse.UpdatedDate = DateTime.UtcNow;
            warehouse.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedWarehouse = await _warehouseService.UpdateWarehouseAsync(warehouse);
            var updatedDto = _mapper.Map<WarehouseDto>(updatedWarehouse);

            _logger.LogInformation("Warehouse updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            _logger.LogInformation("Deleting warehouse ID: {Id}", id);

            var result = await _warehouseService.DeleteWarehouseAsync(id);
            if (!result)
            {
                _logger.LogWarning("Warehouse not found for deletion ID: {Id}", id);
                return NotFound("Warehouse not found");
            }

            _logger.LogInformation("Warehouse deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}