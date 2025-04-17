using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReactBackend.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReactBackend.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ValidateJwtSettings();
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _logger.LogDebug("Generating JWT token for user: {Email}", user.Email);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var keyString = _configuration["JwtSettings:Key"]!;
            var keyBytes = Encoding.UTF8.GetBytes(keyString);

            if (keyBytes.Length < 32)
            {
                _logger.LogError("JWT key is {Length} bytes, but must be at least 32 bytes for HS256", keyBytes.Length);
                throw new InvalidOperationException("JWT key must be at least 32 bytes for HS256.");
            }

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!)),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogDebug("JWT token generated successfully for user: {Email}", user.Email);

            return await Task.FromResult(tokenString);
        }

        private void ValidateJwtSettings()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            if (!jwtSettings.Exists())
            {
                _logger.LogError("JwtSettings section is missing in configuration.");
                throw new InvalidOperationException("JwtSettings section is missing in configuration.");
            }

            if (string.IsNullOrEmpty(jwtSettings["Key"]))
            {
                _logger.LogError("JwtSettings:Key is missing or empty.");
                throw new InvalidOperationException("JwtSettings:Key is required.");
            }

            if (string.IsNullOrEmpty(jwtSettings["Issuer"]))
            {
                _logger.LogError("JwtSettings:Issuer is missing or empty.");
                throw new InvalidOperationException("JwtSettings:Issuer is required.");
            }

            if (string.IsNullOrEmpty(jwtSettings["Audience"]))
            {
                _logger.LogError("JwtSettings:Audience is missing or empty.");
                throw new InvalidOperationException("JwtSettings:Audience is required.");
            }

            if (!int.TryParse(jwtSettings["ExpiryInMinutes"], out var expiry) || expiry <= 0)
            {
                _logger.LogError("JwtSettings:ExpiryInMinutes is invalid or missing.");
                throw new InvalidOperationException("JwtSettings:ExpiryInMinutes must be a positive integer.");
            }
        }
    }
}