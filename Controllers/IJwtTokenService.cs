using ReactBackend.Models;
using System.Threading.Tasks;

namespace ReactBackend.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}