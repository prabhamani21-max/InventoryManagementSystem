using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.DTO;
using InventoryManagementSystem.Service.Implementation;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StoneRateController : ControllerBase
    {
        private readonly IStoneRateService _stoneRateService;
        private readonly ILogger<StoneRateController> _logger;
        private readonly IMapper _mapper;

        private readonly ICurrentUser _currentUser;

        public StoneRateController(
            IStoneRateService stoneRateService,
            IMapper mapper,
            ILogger<StoneRateController> logger,
            ICurrentUser currentUser)
        {
            _stoneRateService = stoneRateService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Get current stone rate by search criteria (4Cs for diamonds, grade for colored stones)
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentRate([FromBody] StoneRateSearchDto searchDto)
        {
            _logger.LogInformation("Getting current stone rate");
            var result = await _stoneRateService.GetCurrentRateBySearchAsync(
                searchDto.StoneId,
                searchDto.Carat,
                searchDto.Cut,
                searchDto.Color,
                searchDto.Clarity,
                searchDto.Grade);

            if (result == null)
            {
                return NotFound("No stone rate found for the given criteria");
            }

            var rateDto = _mapper.Map<StoneRateDto>(result);
            return Ok(rateDto);
        }

        /// <summary>
        /// Get current rate by stone ID
        /// </summary>
        [HttpGet("stone/{stoneId}")]
        public async Task<IActionResult> GetCurrentRateByStoneId(int stoneId)
        {
            _logger.LogInformation("Getting current rate for stone {StoneId}", stoneId);
            var result = await _stoneRateService.GetCurrentRateByStoneIdAsync(stoneId);

            if (result == null)
            {
                return NotFound($"No rate found for stone ID {stoneId}");
            }

            var rateDto = _mapper.Map<StoneRateDto>(result);
            return Ok(rateDto);
        }

        /// <summary>
        /// Get diamond rate by 4Cs (Carat, Cut, Color, Clarity)
        /// </summary>
        [HttpGet("diamond")]
        public async Task<IActionResult> GetDiamondRateBy4Cs(
            [FromQuery] decimal carat,
            [FromQuery] string cut,
            [FromQuery] string color,
            [FromQuery] string clarity)
        {
            _logger.LogInformation("Getting diamond rate for {Carat}ct, {Cut}, {Color}, {Clarity}", carat, cut, color, clarity);
            var result = await _stoneRateService.GetDiamondRateBy4CsAsync(carat, cut, color, clarity);

            if (result == null)
            {
                return NotFound($"No diamond rate found for {carat}ct, {cut}, {color}, {clarity}");
            }

            var rateDto = _mapper.Map<StoneRateDto>(result);
            return Ok(rateDto);
        }

        /// <summary>
        /// Add a new stone rate entry
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddStoneRate([FromBody] StoneRateCreateDto dto)
        {
            _logger.LogInformation("Adding new stone rate for stone {StoneId}", dto.StoneId);

            var stoneRate = _mapper.Map<StoneRateHistory>(dto);
            stoneRate.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            stoneRate.StatusId = (int)StatusEnum.Active;
            stoneRate.CreatedDate = DateTime.UtcNow;

            var result = await _stoneRateService.AddStoneRateAsync(stoneRate);
            var rateDto = _mapper.Map<StoneRateDto>(result);

            _logger.LogInformation("Stone rate added successfully ID: {Id}", rateDto.Id);
            return Ok(rateDto);
        }

        /// <summary>
        /// Update stone rate
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateStoneRate([FromBody] StoneRateUpdateDto dto)
        {
            _logger.LogInformation("Updating stone rate ID {Id}", dto.Id);

            var existingRate = await _stoneRateService.GetRateByIdAsync(dto.Id);
            if (existingRate == null)
            {
                return NotFound($"Stone rate with ID {dto.Id} not found");
            }

            var stoneRate = _mapper.Map<StoneRateHistory>(dto);
            stoneRate.UpdatedDate = DateTime.UtcNow;
            stoneRate.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            var result = await _stoneRateService.UpdateStoneRateAsync(stoneRate);
            var rateDto = _mapper.Map<StoneRateDto>(result);

            _logger.LogInformation("Stone rate updated successfully ID: {Id}", rateDto.Id);
            return Ok(rateDto);
        }

        /// <summary>
        /// Get rate history for a stone
        /// </summary>
        [HttpGet("history/{stoneId}")]
        public async Task<IActionResult> GetRateHistory(int stoneId)
        {
            _logger.LogInformation("Getting rate history for stone {StoneId}", stoneId);
            var history = await _stoneRateService.GetRateHistoryByStoneIdAsync(stoneId);
            var rateDtos = _mapper.Map<IEnumerable<StoneRateDto>>(history);

            return Ok(rateDtos);
        }

        /// <summary>
        /// Get all current stone rates
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCurrentRates()
        {
            _logger.LogInformation("Getting all current stone rates");
            var latestRates = await _stoneRateService.GetAllCurrentRatesAsync();
            var rateDtos = _mapper.Map<IEnumerable<StoneRateDto>>(latestRates);

            return Ok(rateDtos);
        }

        /// <summary>
        /// Get diamond rate card (all 4Cs combinations)
        /// </summary>
        [HttpGet("diamond-rate-card")]
        public async Task<IActionResult> GetDiamondRateCard()
        {
            _logger.LogInformation("Getting diamond rate card");
            var diamondRates = await _stoneRateService.GetDiamondRateCardAsync();
            var rateDtos = _mapper.Map<IEnumerable<StoneRateDto>>(diamondRates);

            return Ok(rateDtos);
        }

        /// <summary>
        /// Get latest rate per unit for Price Calculation Engine
        /// </summary>
        [HttpGet("latest-rate/{stoneId}")]
        public async Task<IActionResult> GetLatestRate(
            int stoneId,
            [FromQuery] decimal? carat,
            [FromQuery] string? cut,
            [FromQuery] string? color,
            [FromQuery] string? clarity,
            [FromQuery] string? grade)
        {
            _logger.LogInformation("Getting latest rate per unit for stone {StoneId}", stoneId);
            var rate = await _stoneRateService.GetLatestRatePerUnitAsync(stoneId, carat, cut, color, clarity, grade);

            return Ok(rate);
        }
    }
}
