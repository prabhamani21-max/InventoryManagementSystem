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
    public class MetalRateController : ControllerBase
    {
        private readonly IMetalRateService _metalRateService;
        private readonly IMetalService _metalService;
        private readonly IPurityService _purityService;
        private readonly IMapper _mapper;
        private readonly ILogger<MetalRateController> _logger;
        private readonly ICurrentUser _currentUser;

        public MetalRateController(
            IMetalRateService metalRateService,
            IMetalService metalService,
            IPurityService purityService,
            IMapper mapper,
            ILogger<MetalRateController> logger,
            ICurrentUser currentUser)
        {
            _metalRateService = metalRateService;
            _metalService = metalService;
            _purityService = purityService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Get current rate for a specific purity
        /// </summary>
        [HttpGet("current/purity/{purityId}")]
        public async Task<IActionResult> GetCurrentRateByPurity(int purityId)
        {
            _logger.LogInformation("Getting current rate for purity {PurityId}", purityId);
            var rate = await _metalRateService.GetCurrentRateByPurityIdAsync(purityId);

            if (rate == null)
            {
                return NotFound();
            }

            var response = await MapToMetalRateResponseDto(rate, purityId);
            return Ok(response);
        }

        /// <summary>
        /// Get current rates for all purities of a metal
        /// </summary>
        [HttpGet("current/metal/{metalId}")]
        public async Task<IActionResult> GetCurrentRatesByMetal(int metalId)
        {
            _logger.LogInformation("Getting current rates for metal {MetalId}", metalId);
            var rates = await _metalRateService.GetCurrentRatesByMetalIdAsync(metalId);

            var responses = new List<MetalRateResponseDto>();
            foreach (var rate in rates)
            {
                var response = await MapToMetalRateResponseDto(rate, rate.PurityId);
                responses.Add(response);
            }

            return Ok(responses);
        }

        /// <summary>
        /// Get all current rates across all metals
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetAllCurrentRates()
        {
            _logger.LogInformation("Getting all current rates");
            var rates = await _metalRateService.GetAllCurrentRatesAsync();

            var responses = new List<MetalRateResponseDto>();
            foreach (var rate in rates)
            {
                var response = await MapToMetalRateResponseDto(rate, rate.PurityId);
                responses.Add(response);
            }

            return Ok(responses);
        }

        /// <summary>
        /// Add a new metal rate entry
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddMetalRate([FromBody] MetalRateCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new metal rate for purity {PurityId}: {Rate}", dto.PurityId, dto.RatePerGram);

            var metalRate = new MetalRateHistory
            {
                PurityId = dto.PurityId,
                RatePerGram = dto.RatePerGram,
                EffectiveDate = dto.EffectiveDate,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin,
                StatusId = 1 // Active status
            };

            var result = await _metalRateService.AddMetalRateAsync(metalRate);

            var response = await MapToMetalRateDto(result);

            return CreatedAtAction(nameof(GetCurrentRateByPurity), new { purityId = result.PurityId }, response);
        }

        /// <summary>
        /// Get rate history for a specific purity
        /// </summary>
        [HttpGet("history/purity/{purityId}")]
        public async Task<IActionResult> GetRateHistoryByPurity(int purityId)
        {
            _logger.LogInformation("Getting rate history for purity {PurityId}", purityId);
            var history = await _metalRateService.GetRateHistoryByPurityIdAsync(purityId);

            var responses = new List<MetalRateHistoryDto>();
            foreach (var rate in history)
            {
                responses.Add(await MapToMetalRateHistoryDto(rate, purityId));
            }

            return Ok(responses);
        }

        /// <summary>
        /// Get rate history for a metal within a date range
        /// </summary>
        [HttpGet("history/metal/{metalId}")]
        public async Task<IActionResult> GetRateHistoryByMetal(int metalId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            _logger.LogInformation("Getting rate history for metal {MetalId} from {StartDate} to {EndDate}", metalId, startDate, endDate);
            
            if (startDate == default || endDate == default)
            {
                startDate = DateTime.UtcNow.AddDays(-30);
                endDate = DateTime.UtcNow;
            }

            var history = await _metalRateService.GetRateHistoryByMetalIdAsync(metalId, startDate, endDate);

            var responses = new List<MetalRateHistoryDto>();
            foreach (var rate in history)
            {
                responses.Add(await MapToMetalRateHistoryDto(rate, rate.PurityId));
            }

            return Ok(responses);
        }

        /// <summary>
        /// Update metal rate
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateMetalRate([FromBody] MetalRateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating metal rate ID {Id}: {Rate}", dto.Id, dto.RatePerGram);

            var existingRate = await _metalRateService.GetRateByIdAsync(dto.Id);
            if (existingRate == null)
            {
                return NotFound();
            }

            existingRate.RatePerGram = dto.RatePerGram;
            existingRate.EffectiveDate = dto.EffectiveDate;
            existingRate.UpdatedDate = DateTime.UtcNow;
            existingRate.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var result = await _metalRateService.UpdateMetalRateAsync(existingRate);

            var response = await MapToMetalRateDto(result);

            return Ok(response);
        }

        private async Task<MetalRateResponseDto> MapToMetalRateResponseDto(MetalRateHistory rate, int purityId)
        {
            var response = _mapper.Map<MetalRateResponseDto>(rate);
            var purity = await _purityService.GetPurityByIdAsync(purityId);
            var metal = purity != null ? await _metalService.GetMetalByIdAsync(purity.MetalId) : null;
            return new MetalRateResponseDto
            {
                PurityId = purityId,
                PurityName = purity?.Name ?? string.Empty,
                MetalId = purity?.MetalId ?? 0,
                MetalName = metal?.Name ?? string.Empty,
                Percentage = purity?.Percentage ?? 0,
                CurrentRatePerGram = rate.RatePerGram,
                EffectiveDate = rate.EffectiveDate,
                LastUpdated = rate.CreatedDate
            };
        }

        private async Task<MetalRateDto> MapToMetalRateDto(MetalRateHistory rate)
        {
            var purity = await _purityService.GetPurityByIdAsync(rate.PurityId);
            var metal = purity != null ? await _metalService.GetMetalByIdAsync(purity.MetalId) : null;
            return new MetalRateDto
            {
                Id = rate.Id,
                PurityId = rate.PurityId,
                PurityName = purity?.Name,
                MetalId = purity?.MetalId ?? 0,
                MetalName = metal?.Name ?? string.Empty,
                RatePerGram = rate.RatePerGram,
                EffectiveDate = rate.EffectiveDate,
                CreatedDate = rate.CreatedDate,
                StatusId = rate.StatusId
            };
        }

        private async Task<MetalRateHistoryDto> MapToMetalRateHistoryDto(MetalRateHistory rate, int purityId)
        {
            var purity = await _purityService.GetPurityByIdAsync(purityId);
            return new MetalRateHistoryDto
            {
                Id = rate.Id,
                PurityId = purityId,
                PurityName = purity?.Name ?? string.Empty,
                RatePerGram = rate.RatePerGram,
                EffectiveDate = rate.EffectiveDate,
                CreatedDate = rate.CreatedDate
            };
        }
    }
}
