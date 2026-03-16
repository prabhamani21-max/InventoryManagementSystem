using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Common.Models;

namespace SalexiHRSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;
        private readonly ICurrentUser _currentUser;

        public RoleController(
            IRoleService roleService,
            IStatusService statusService,
            IMapper mapper,
            ILogger<RoleController> logger, ICurrentUser currentUser)
        {
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        // ---------------------- Role Endpoints ----------------------

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            _logger.LogInformation("Fetching all roles.");
            var roles = await _roleService.GetAllRoles();
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            _logger.LogInformation("Successfully fetched {Count} roles.", roleDtos.Count());
            return Ok(roleDtos);
        }

        [Authorize]
        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            _logger.LogInformation("Fetching role with ID {Id}.", id);
            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {Id} not found.", id);
                return NotFound("Role not found.");
            }
            var roleDto = _mapper.Map<RoleDto>(role);
            return Ok(roleDto);
        }

        [Authorize]
        [HttpPost("AddEdit")]
        public async Task<IActionResult> AddEditRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for role DTO.");
                return BadRequest(ModelState);
            }

            if (roleDto.Id == null || roleDto.Id == 0)
            {
                if (await IsDuplicateRoleDataAsync(roleDto.Name))
                {
                    _logger.LogWarning("Lead status name '{Name}' already exists.", roleDto.Name);
                    return Conflict("Duplicate lead status name exists in the database.");
                }
            }
            else
            {
                var existingWithSameName = await _roleService.GetByName(roleDto.Name);
                if (existingWithSameName != null && existingWithSameName.Id != roleDto.Id)
                {
                    _logger.LogWarning("Lead status name '{Name}' already exists in another record.", roleDto.Name);
                    return Conflict("Duplicate lead status name exists in another record.");
                }
            }

            _logger.LogInformation("Adding/Editing role with ID {Id}.", roleDto.Id);
            var role = _mapper.Map<Role>(roleDto);
            role.CreatedBy = _currentUser.UserId;
            role.UpdatedBy = _currentUser.UserId;
            var result = await _roleService.AddEditRole(role);
            var resultDto = _mapper.Map<RoleDto>(result);

            if (roleDto.Id == null || roleDto.Id == 0)
            {
                _logger.LogInformation("Created role with ID {Id}.", resultDto.Id);
                return CreatedAtAction(nameof(GetRoleById), new { id = resultDto.Id }, resultDto);
            }

            _logger.LogInformation("Updated role with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            _logger.LogInformation("Deleting role with ID {Id}.", id);
            var result = await _roleService.DeleteRole(id);
            if (result == null)
            {
                _logger.LogWarning("Role with ID {Id} not found.", id);
                return NotFound("Role not found.");
            }
            var resultDto = _mapper.Map<RoleDto>(result);
            _logger.LogInformation("Deleted role with ID {Id}.", id);
            return Ok(resultDto);
        }      

        private async Task<bool> IsDuplicateRoleDataAsync(string name)
        {
            name = name?.Trim().ToLower();
                    var existingRole = await _roleService.GetByName(name);
                    return existingRole != null;

        }

    }
}