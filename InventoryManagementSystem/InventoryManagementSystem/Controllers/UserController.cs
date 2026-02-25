using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.DTO;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;


        public UserController(IJwtService jwtService, IUserService userService,IMapper mapper, ILogger<UserController> logger, ICurrentUser currentUser)
        {
            _jwtService = jwtService;
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
            _currentUser = currentUser;
          }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for identifier: {Identifier}", dto.Identifier);

            UserDb userDb = null;
            if (dto.Identifier.Contains("@"))
            {
                // It's an email
                userDb = await _userService.GetUserByEmailAsync(dto.Identifier);
            }
            else
            {
                // It's a phone number
                userDb = await _userService.GetUserByContactNumberAsync(dto.Identifier);
            }

            if (userDb == null)
            {
                _logger.LogError("Enter the proper details");
                return NotFound("Invalid Email or Phone Number");
            }
            if (userDb.StatusId == (int)StatusEnum.Inactive || userDb.StatusId == (int)StatusEnum.Deleted)
            {
                _logger.LogInformation("Login failed - Status is Not Active: {Identifier}", dto.Identifier);
                return Forbid("User is inactive");
            }

            // Hash the incoming password and compare with DB value
            var hashedInputPassword = PasswordHasher.HashPassword(dto.Password);
            if (!string.Equals(userDb.Password, hashedInputPassword, StringComparison.Ordinal))
            {
                _logger.LogError("Login failed - Incorrect password for: {Identifier}", dto.Identifier);
                return Unauthorized("Invalid password.");
            }
                var user = _mapper.Map<User>(userDb);
                var token = _jwtService.GenerateToken(user);
                _logger.LogInformation("Login Successfull");
                return Ok(new
                {
                    Token = token,
                });
}
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Check email existence
            if (await _userService.EmailExistsAsync(dto.Email))
            {
                // Return conflict with field-specific error
                // Middleware will wrap this with Status=false and standard format
                ModelState.AddModelError("Email", "Email already registered");
                return Conflict(ModelState);
            }

            // Check contact number existence
            if (await _userService.ContactNumberExistsAsync(dto.ContactNumber))
            {
                // Return conflict with field-specific error
                ModelState.AddModelError("ContactNo", "Contact number already registered");
                return Conflict(ModelState);
            }

            var user = _mapper.Map<User>(dto);

            user.CreatedDate = DateTime.UtcNow;
            user.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            user.StatusId = (int)StatusEnum.Active;
            user.Password = PasswordHasher.HashPassword(user.Password);

            var registeredUser = await _userService.RegisterUserAsync(user);
            var userDto = _mapper.Map<UserDto>(registeredUser);
            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            // await _userPublisherService.PublishUserAdded(newUser);
            return Ok(new { Message = "User registered successfully", User = userDto });
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users");
            var users = await _userService.GetAllUsersAsync();
            var userDto = _mapper .Map<IEnumerable<UserDto>>(users);
            return Ok(userDto);
        }

        [Authorize]
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            _logger.LogInformation("Fetching user with ID: {Id}", id);
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {Id}", id);
                return NotFound($"User with ID {id} not found");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }
    }
}