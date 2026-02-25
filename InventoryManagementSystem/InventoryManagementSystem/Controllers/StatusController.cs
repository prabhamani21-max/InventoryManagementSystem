using AutoMapper;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Service.Implementation;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalexiHRSystem.Controllers;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;
        private readonly ICurrentUser _currentUser;
        public StatusController(IStatusService statusService,
            IMapper mapper,
            ILogger<RoleController> logger, 
            ICurrentUser currentUser)
        {
            _statusService = statusService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }
        // ---------------------- User Status Endpoints ----------------------

        [Authorize]
        [HttpGet("userStatuses/GetAllUserStatuses")]
        public async Task<IActionResult> GetAllUserStatuses()
        {
            _logger.LogInformation("Fetching all user statuses.");
            var userStatuses = await _statusService.GetAllUserStatuses();
            var userStatusDtos = _mapper.Map<IEnumerable<StatusDto>>(userStatuses);
            _logger.LogInformation("Successfully fetched {Count} user statuses.", userStatusDtos.Count());
            return Ok(userStatusDtos);
        }

        [Authorize]
        [HttpGet("userStatuses/GetById/{id}")]
        public async Task<IActionResult> GetUserStatusById(int id)
        {
            _logger.LogInformation("Fetching user status with ID {Id}.", id);
            var userStatus = await _statusService.GetUserStatusById(id);
            if (userStatus == null)
            {
                _logger.LogWarning("User status with ID {Id} not found.", id);
                return NotFound("User status not found.");
            }
            var userStatusDto = _mapper.Map<StatusDto>(userStatus);
            return Ok(userStatusDto);
        }

        [Authorize]
        [HttpPost("userStatuses/AddEdit")]
        public async Task<IActionResult> AddEditUserStatus([FromBody] StatusDto userStatusDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user status DTO.");
                return BadRequest(ModelState);
            }

            if (userStatusDto.Id == null || userStatusDto.Id == 0)
            {
                if (await IsDuplicateStatusAsync(userStatusDto.Name))
                {
                    _logger.LogWarning("Lead status name '{Name}' already exists.", userStatusDto.Name);
                    return Conflict("Duplicate lead status name exists in the database.");
                }
            }
            else
            {
                var existingWithSameName = await _statusService.GetByName(userStatusDto.Name);
                if (existingWithSameName != null && existingWithSameName.Id != userStatusDto.Id)
                {
                    _logger.LogWarning("Lead status name '{Name}' already exists in another record.", userStatusDto.Name);
                    return Conflict("Duplicate lead status name exists in another record.");
                }
            }

            _logger.LogInformation("Adding/Editing user status with ID {Id}.", userStatusDto.Id);
            var userStatus = _mapper.Map<GenericStatus>(userStatusDto);
            userStatus.CreatedBy = _currentUser.UserId;
            userStatus.UpdatedBy = _currentUser.UserId;
            var result = await _statusService.AddEditUserStatus(userStatus);
            var resultDto = _mapper.Map<StatusDto>(result);

            if (userStatusDto.Id == null || userStatusDto.Id == 0)
            {
                _logger.LogInformation("Created user status with ID {Id}.", resultDto.Id);
                return CreatedAtAction(nameof(GetUserStatusById), new { id = resultDto.Id }, resultDto);
            }

            _logger.LogInformation("Updated user status with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("userStatuses/Delete/{id}")]
        public async Task<IActionResult> DeleteUserStatus(int id)
        {
            _logger.LogInformation("Deleting user status with ID {Id}.", id);
            var result = await _statusService.DeleteUserStatus(id);
            if (result == null)
            {
                _logger.LogWarning("User status with ID {Id} not found.", id);
                return NotFound("User status not found.");
            }
            var resultDto = _mapper.Map<StatusDto>(result);
            _logger.LogInformation("Deleted user status with ID {Id}.", id);
            return Ok(resultDto);
        }
        private async Task<bool> IsDuplicateStatusAsync(string name)
        {
            name = name?.Trim().ToLower();
            var existingStatus = await _statusService.GetByName(name);
            return existingStatus != null;

        }

    }
}
