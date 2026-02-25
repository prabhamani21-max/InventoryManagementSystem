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
    public class UserKycController : ControllerBase
    {
        private readonly IUserKycService _userKycService;
        private readonly ILogger<UserKycController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public UserKycController(IUserKycService userKycService, IMapper mapper, ILogger<UserKycController> logger, ICurrentUser currentUser)
        {
            _userKycService = userKycService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserKycs()
        {
            _logger.LogInformation("Fetching all user KYC records");
            var userKycs = await _userKycService.GetAllUserKycsAsync();
            var userKycDtos = _mapper.Map<IEnumerable<UserKycDto>>(userKycs);
            return Ok(userKycDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserKycById(long id)
        {
            _logger.LogInformation("Fetching user KYC by ID: {Id}", id);
            var userKyc = await _userKycService.GetUserKycByIdAsync(id);
            if (userKyc == null)
            {
                _logger.LogWarning("User KYC not found for ID: {Id}", id);
                return NotFound("User KYC not found");
            }
            var userKycDto = _mapper.Map<UserKycDto>(userKyc);
            return Ok(userKycDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserKycByUserId(long userId)
        {
            _logger.LogInformation("Fetching user KYC by User ID: {UserId}", userId);
            var userKyc = await _userKycService.GetUserKycByUserIdAsync(userId);
            if (userKyc == null)
            {
                _logger.LogWarning("User KYC not found for User ID: {UserId}", userId);
                return NotFound("User KYC not found");
            }
            var userKycDto = _mapper.Map<UserKycDto>(userKyc);
            return Ok(userKycDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserKyc([FromBody] UserKycDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating user KYC for User ID: {UserId}", dto.UserId);
            // 1️⃣ Check if KYC already exists for this user
            if (await _userKycService.UserKycExistsAsync(dto.UserId))
            {
                return Conflict(new
                {
                    message = "KYC already exists for this user"
                });
            }


            // Check PAN card uniqueness
            if (!string.IsNullOrEmpty(dto.PanCardNumber) && await _userKycService.PanCardNumberExistsAsync(dto.PanCardNumber))
            {
                ModelState.AddModelError("PanCardNumber", "PAN card number already exists");
                return Conflict(ModelState);
            }

            // Check Aadhaar card uniqueness
            if (!string.IsNullOrEmpty(dto.AadhaarCardNumber) && await _userKycService.AadhaarCardNumberExistsAsync(dto.AadhaarCardNumber))
            {
                ModelState.AddModelError("AadhaarCardNumber", "Aadhaar card number already exists");
                return Conflict(ModelState);
            }

            var userKyc = _mapper.Map<UserKyc>(dto);
            userKyc.CreatedDate = DateTime.UtcNow;
            userKyc.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            userKyc.StatusId = (int)StatusEnum.Active;

            var createdUserKyc = await _userKycService.CreateUserKycAsync(userKyc);
            var responseDto = _mapper.Map<UserKycDto>(createdUserKyc);

            _logger.LogInformation("User KYC created successfully for User ID: {UserId}", dto.UserId);
            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserKyc(long id, [FromBody] UserKycDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating user KYC ID: {Id}", id);

            var existingUserKyc = await _userKycService.GetUserKycByIdAsync(id);
            if (existingUserKyc == null)
            {
                _logger.LogWarning("User KYC not found for update ID: {Id}", id);
                return NotFound("User KYC not found");
            }
            //Check UserId uniqueness exclusing current record
            if (existingUserKyc.UserId != dto.UserId)
            {
                if (await _userKycService.UserKycExistsAsync(dto.UserId, id))
                {
                    return Conflict(new { message = "KYC already exists for this user" });
                }
            }

            // Check PAN card uniqueness excluding current record
            if (!string.IsNullOrEmpty(dto.PanCardNumber) && await _userKycService.PanCardNumberExistsAsync(dto.PanCardNumber, id))
            {
                ModelState.AddModelError("PanCardNumber", "PAN card number already exists");
                return Conflict(ModelState);
            }

            // Check Aadhaar card uniqueness excluding current record
            if (!string.IsNullOrEmpty(dto.AadhaarCardNumber) && await _userKycService.AadhaarCardNumberExistsAsync(dto.AadhaarCardNumber, id))
            {
                ModelState.AddModelError("AadhaarCardNumber", "Aadhaar card number already exists");
                return Conflict(ModelState);
            }

            var userKyc = _mapper.Map<UserKyc>(dto);
            userKyc.Id = id;
            userKyc.UpdatedDate = DateTime.UtcNow;
            userKyc.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedUserKyc = await _userKycService.UpdateUserKycAsync(userKyc);
            var updatedDto = _mapper.Map<UserKycDto>(updatedUserKyc);

            _logger.LogInformation("User KYC updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserKyc(long id)
        {
            _logger.LogInformation("Deleting user KYC ID: {Id}", id);

            var result = await _userKycService.DeleteUserKycAsync(id);
            if (!result)
            {
                _logger.LogWarning("User KYC not found for deletion ID: {Id}", id);
                return NotFound("User KYC not found");
            }

            _logger.LogInformation("User KYC deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}