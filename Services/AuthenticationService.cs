using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GiacenzaSorterRm.Models;
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IActiveDirectoryService _adService;
        private readonly IMemoryCache _cache;
        private readonly AuthenticationSettings _settings;

        private const string LOCKOUT_KEY_PREFIX = "lockout_";
        private const string FAILED_ATTEMPTS_KEY_PREFIX = "failed_attempts_";

        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IActiveDirectoryService adService,
            IMemoryCache cache,
            IOptions<AuthenticationSettings> settings)
        {
            _logger = logger;
            _adService = adService;
            _cache = cache;
            _settings = settings.Value;
        }

        public async Task<bool> AuthenticateAsync(Operatori user, string password)
        {
            if (user == null)
            {
                _logger.LogWarning("Authentication attempt with null user");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Authentication attempt with empty password for user: {Username}", 
                    user.Operatore);
                return false;
            }

            // Verifica lockout
            if (await IsAccountLockedAsync(user.Operatore))
            {
                _logger.LogWarning("Authentication blocked - account locked: {Username}", 
                    user.Operatore);
                return false;
            }

            try
            {
                bool isAuthenticated = false;

                if (user.Azienda == "ESTERNO")
                {
                    // Utenti ESTERNI: solo autenticazione database
                    isAuthenticated = await AuthenticateExternalUserAsync(user, password);
                }
                else
                {
                    // Utenti INTERNI: prova prima AD, poi fallback su database
                    _logger.LogDebug("Attempting AD authentication for internal user: {Username}", user.Operatore);
                    isAuthenticated = await _adService.AuthenticateAsync(user.Operatore, password);
                    
                    // Fallback su database se AD fallisce e l'utente ha password hash nel DB
                    if (!isAuthenticated && !string.IsNullOrEmpty(user.Password))
                    {
                        _logger.LogInformation("AD authentication failed for {Username}, attempting database fallback", 
                            user.Operatore);
                        isAuthenticated = await AuthenticateExternalUserAsync(user, password);
                        
                        if (isAuthenticated)
                        {
                            _logger.LogInformation("Database fallback authentication successful for internal user: {Username}", 
                                user.Operatore);
                        }
                    }
                }

                if (isAuthenticated)
                {
                    // Reset contatore tentativi falliti
                    _cache.Remove(GetFailedAttemptsKey(user.Operatore));
                    _logger.LogInformation("Successful authentication for user: {Username}", 
                        user.Operatore);
                }
                else
                {
                    await RecordFailedAttemptAsync(user.Operatore);
                }

                return isAuthenticated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication error for user: {Username}", user.Operatore);
                return false;
            }
        }

        private async Task<bool> AuthenticateExternalUserAsync(Operatori user, string password)
        {
            return await Task.Run(() =>
            {
                var passwordHasher = new PasswordHasher<string>();
                var state = passwordHasher.VerifyHashedPassword(null, user.Password, password);
                
                return state == PasswordVerificationResult.Success || 
                       state == PasswordVerificationResult.SuccessRehashNeeded;
            });
        }

        public async Task<bool> IsAccountLockedAsync(string username)
        {
            var key = GetLockoutKey(username);
            return await Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public async Task RecordFailedAttemptAsync(string username)
        {
            var key = GetFailedAttemptsKey(username);
            
            if (!_cache.TryGetValue(key, out int failedAttempts))
            {
                failedAttempts = 0;
            }

            failedAttempts++;

            _cache.Set(key, failedAttempts, TimeSpan.FromMinutes(_settings.LockoutMinutes));

            _logger.LogWarning("Failed login attempt {Attempt}/{Max} for user: {Username}", 
                failedAttempts, _settings.MaxFailedAttempts, username);

            if (failedAttempts >= _settings.MaxFailedAttempts)
            {
                var lockoutKey = GetLockoutKey(username);
                _cache.Set(lockoutKey, true, TimeSpan.FromMinutes(_settings.LockoutMinutes));
                
                _logger.LogWarning("Account locked for {Minutes} minutes: {Username}", 
                    _settings.LockoutMinutes, username);
            }

            await Task.CompletedTask;
        }

        private string GetLockoutKey(string username) => 
            $"{LOCKOUT_KEY_PREFIX}{username.ToLowerInvariant()}";

        private string GetFailedAttemptsKey(string username) => 
            $"{FAILED_ATTEMPTS_KEY_PREFIX}{username.ToLowerInvariant()}";
    }
}
