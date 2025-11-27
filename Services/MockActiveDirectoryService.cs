using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GiacenzaSorterRm.Services
{
    /// <summary>
    /// Implementazione mock di IActiveDirectoryService per sviluppo locale senza AD
    /// </summary>
    public class MockActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ILogger<MockActiveDirectoryService> _logger;

        public MockActiveDirectoryService(ILogger<MockActiveDirectoryService> logger)
        {
            _logger = logger;
            _logger.LogWarning("Using MockActiveDirectoryService - AD authentication is DISABLED");
        }

        public Task<bool> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation("Mock AD authentication for user: {Username} - Always returns false", username);
            return Task.FromResult(false);
        }

        public Task<bool> UserExistsAsync(string username)
        {
            _logger.LogInformation("Mock AD UserExists check for: {Username} - Always returns false", username);
            return Task.FromResult(false);
        }

        public Task<bool> IsAccountEnabledAsync(string username)
        {
            _logger.LogInformation("Mock AD IsAccountEnabled check for: {Username} - Always returns false", username);
            return Task.FromResult(false);
        }
    }
}
