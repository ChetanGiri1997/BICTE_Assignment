using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactBackend.Data;
using ReactBackend.DTOs;
using ReactBackend.Models;
using System.Security.Claims;

namespace ReactBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReports()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Get reports failed: User not authenticated.");
                return Unauthorized(new ErrorResponseDto { Message = "User not authenticated" });
            }

            var reports = await _context.Reports
                .Where(r => r.UserId == userId)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} reports for user: {UserId}", reports.Count, userId);
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDto>> GetReport(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Get report failed: User not authenticated.");
                return Unauthorized(new ErrorResponseDto { Message = "User not authenticated" });
            }

            var report = await _context.Reports
                .Where(r => r.Id == id && r.UserId == userId)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                })
                .FirstOrDefaultAsync();

            if (report == null)
            {
                _logger.LogWarning("Report not found with ID: {Id} for user: {UserId}", id, userId);
                return NotFound(new ErrorResponseDto { Message = "Report not found" });
            }

            _logger.LogInformation("Retrieved report with ID: {Id} for user: {UserId}", id, userId);
            return Ok(report);
        }

        [HttpPost]
        public async Task<ActionResult<ReportDto>> CreateReport([FromBody] CreateReportDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Invalid model state for create report request: {Errors}", string.Join(", ", errors));
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid report data",
                    Errors = errors
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Create report failed: User not authenticated.");
                return Unauthorized(new ErrorResponseDto { Message = "User not authenticated" });
            }

            var report = new Report
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Reports.Add(report);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create report for user: {UserId}", userId);
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while creating the report" });
            }

            var reportDto = new ReportDto
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                CreatedAt = report.CreatedAt,
                UserId = report.UserId
            };

            _logger.LogInformation("Created report with ID: {Id} for user: {UserId}", report.Id, userId);
            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, reportDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] CreateReportDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Invalid model state for update report request: {Errors}", string.Join(", ", errors));
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid report data",
                    Errors = errors
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Update report failed: User not authenticated.");
                return Unauthorized(new ErrorResponseDto { Message = "User not authenticated" });
            }

            var report = await _context.Reports
                .Where(r => r.Id == id && r.UserId == userId)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                _logger.LogWarning("Report not found with ID: {Id} for user: {UserId}", id, userId);
                return NotFound(new ErrorResponseDto { Message = "Report not found" });
            }

            report.Title = model.Title;
            report.Description = model.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update report with ID: {Id} for user: {UserId}", id, userId);
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while updating the report" });
            }

            _logger.LogInformation("Updated report with ID: {Id} for user: {UserId}", id, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Delete report failed: User not authenticated.");
                return Unauthorized(new ErrorResponseDto { Message = "User not authenticated" });
            }

            var report = await _context.Reports
                .Where(r => r.Id == id && r.UserId == userId)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                _logger.LogWarning("Report not found with ID: {Id} for user: {UserId}", id, userId);
                return NotFound(new ErrorResponseDto { Message = "Report not found" });
            }

            _context.Reports.Remove(report);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete report with ID: {Id} for user: {UserId}", id, userId);
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while deleting the report" });
            }

            _logger.LogInformation("Deleted report with ID: {Id} for user: {UserId}", id, userId);
            return NoContent();
        }
    }
}