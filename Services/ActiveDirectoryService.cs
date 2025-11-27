using System;
using System.DirectoryServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GiacenzaSorterRm.Models;

namespace GiacenzaSorterRm.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly ActiveDirectorySettings _settings;

        private const int ADS_UF_ACCOUNTDISABLE = 0x0002;

        public ActiveDirectoryService(
            ILogger<ActiveDirectoryService> logger,
            IOptions<ActiveDirectorySettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(_settings?.LdapPath))
            {
                throw new InvalidOperationException(
                    "Active Directory LDAP path not configured. " +
                    "Add 'ActiveDirectory:LdapPath' to appsettings.json");
            }

            if (_settings.ServiceAccount == null || 
                string.IsNullOrEmpty(_settings.ServiceAccount.Username))
            {
                throw new InvalidOperationException(
                    "Active Directory Service Account not configured. " +
                    "Add 'ActiveDirectory:ServiceAccount' credentials to User Secrets");
            }
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("AD authentication attempt with empty credentials");
                return false;
            }

            return await Task.Run(() => AuthenticateInternal(username, password));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Convalida compatibilità della piattaforma", Justification = "<In sospeso>")]
        private bool AuthenticateInternal(string username, string password)
        {
            DirectoryEntry searchRoot = null;
            DirectorySearcher searcher = null;
            DirectoryEntry userEntry = null;

            try
            {
                var authType = _settings.UseServerBinding 
                    ? AuthenticationTypes.ServerBind | AuthenticationTypes.Secure
                    : AuthenticationTypes.Secure;

                // FASE 1: Bind con service account per cercare l'utente
                searchRoot = new DirectoryEntry(
                    _settings.LdapPath,
                    _settings.ServiceAccount.Username,
                    _settings.ServiceAccount.Password,
                    authType);


                // FASE 2: Cerca utente nel dominio
                searcher = new DirectorySearcher(searchRoot)
                {
                    Filter = $"(&(objectClass=user)(sAMAccountName={EscapeLdapFilter(username)}))",
                    PropertiesToLoad = { "distinguishedName", "userAccountControl", "cn" },
                    SearchScope = SearchScope.Subtree,
                    ServerTimeLimit = TimeSpan.FromSeconds(_settings.TimeoutSeconds)
                };

                var result = searcher.FindOne();
                
                if (result == null)
                {
                    _logger.LogWarning("User not found in Active Directory: {Username}", username);
                    return false;
                }

                // FASE 3: Verifica che l'account sia attivo
                if (result.Properties["userAccountControl"].Count > 0)
                {
                    var userAccountControl = (int)result.Properties["userAccountControl"][0];
                    
                    if ((userAccountControl & ADS_UF_ACCOUNTDISABLE) != 0)
                    {
                        _logger.LogWarning("Disabled AD account login attempt: {Username}", username);
                        return false;
                    }
                }

                // FASE 4: Valida password con secondo bind
                string userDn = result.Properties["distinguishedName"][0].ToString();
                
                userEntry = new DirectoryEntry(
                    $"{_settings.LdapPath}/{userDn}",
                    username,
                    password,
                    authType);

                // Forza bind per validare credenziali
                object nativeObject = userEntry.NativeObject;
                
                _logger.LogInformation("Successful AD authentication: {Username}", username);
                return true;
            }
            catch (DirectoryServicesCOMException ex)
            {
                _logger.LogWarning(ex, 
                    "AD authentication failed for user: {Username}. Error: {HResult}", 
                    username, ex.HResult);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Unexpected AD error for user: {Username}", 
                    username);
                return false;
            }
            finally
            {
                userEntry?.Dispose();
                searcher?.Dispose();
                searchRoot?.Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Convalida compatibilità della piattaforma", Justification = "<In sospeso>")]
        public async Task<bool> UserExistsAsync(string username)
        {
            return await Task.Run(() =>
            {
                using var searchRoot = CreateServiceAccountEntry();
                using var searcher = new DirectorySearcher(searchRoot)
                {
                    Filter = $"(&(objectClass=user)(sAMAccountName={EscapeLdapFilter(username)}))",
                    SearchScope = SearchScope.Subtree
                };

                var result = searcher.FindOne();
                return result != null;
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Convalida compatibilità della piattaforma", Justification = "<In sospeso>")]
        public async Task<bool> IsAccountEnabledAsync(string username)
        {
            return await Task.Run(() =>
            {
                using var searchRoot = CreateServiceAccountEntry();
                using var searcher = new DirectorySearcher(searchRoot)
                {
                    Filter = $"(&(objectClass=user)(sAMAccountName={EscapeLdapFilter(username)}))",
                    PropertiesToLoad = { "userAccountControl" },
                    SearchScope = SearchScope.Subtree
                };

                var result = searcher.FindOne();
                
                if (result == null || result.Properties["userAccountControl"].Count == 0)
                {
                    return false;
                }

                var userAccountControl = (int)result.Properties["userAccountControl"][0];
                return (userAccountControl & ADS_UF_ACCOUNTDISABLE) == 0;
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Convalida compatibilità della piattaforma", Justification = "<In sospeso>")]
        private DirectoryEntry CreateServiceAccountEntry()
        {
            var authType = _settings.UseServerBinding 
                ? AuthenticationTypes.ServerBind | AuthenticationTypes.Secure
                : AuthenticationTypes.Secure;

            return new DirectoryEntry(
                _settings.LdapPath,
                _settings.ServiceAccount.Username,
                _settings.ServiceAccount.Password,
                authType);
        }

        /// <summary>
        /// Protegge da LDAP Injection escapando caratteri speciali
        /// </summary>
        private string EscapeLdapFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return filter;

            return filter
                .Replace("\\", "\\5c")
                .Replace("*", "\\2a")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\0", "\\00");
        }
    }
}
