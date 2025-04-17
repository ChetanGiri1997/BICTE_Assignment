using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReactBackend.DTOs;
using ReactBackend.Models;
using ReactBackend.Services;
using System.Threading.Tasks;

namespace ReactBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="model">The user registration details.</param>
        /// <returns>A success message or validation errors.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Invalid model state for register request: {Errors}", string.Join(", ", errors));
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid registration data",
                    Errors = errors
                });
            }

            _logger.LogInformation("Attempting to register user with email: {Email}", model.Email);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName?.Trim()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User registration failed for email: {Email}. Errors: {Errors}",
                    model.Email, string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Registration failed",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }

            _logger.LogInformation("User registered successfully: {Email}", model.Email);
            return Ok(new SuccessResponseDto
            {
                Message = "User registered successfully"
            });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token and user ID.
        /// </summary>
        /// <param name="model">The user login credentials.</param>
        /// <returns>A JWT token, user ID, or an error message.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login request: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid login data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            _logger.LogInformation("Attempting login for email: {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", model.Email);
                return Unauthorized(new ErrorResponseDto
                {
                    Message = "Invalid credentials"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", model.Email);
                return Unauthorized(new ErrorResponseDto
                {
                    Message = "Invalid credentials"
                });
            }

            try
            {
                var token = await _jwtTokenService.GenerateTokenAsync(user);
                _logger.LogInformation("Login successful for email: {Email}", model.Email);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    UserId = user.Id // Include user ID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate JWT token for email: {Email}", model.Email);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An error occurred during login"
                });
            }
        }
    }
}