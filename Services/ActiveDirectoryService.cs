using DocumentFormat.OpenXml.Spreadsheet;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

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


        private bool AuthenticateInternal(string username, string password)
        {
            try
            {
                // 1. Definiamo il contesto del dominio
                // Non passiamo il service account qui: usiamo il contesto del server 
                // o la risoluzione automatica che ha funzionato nel tuo test.
                using (var context = new PrincipalContext(ContextType.Domain, _settings.Domain))
                {
                    // 2. Validazione DIRETTA delle credenziali dell'utente
                    // Questo metodo effettua internamente il bind LDAP necessario.
                    bool isValid = context.ValidateCredentials(username, password);

                    if (!isValid)
                    {
                        _logger.LogWarning("Credenziali non valide o account bloccato per: {Username}", username);
                        return false;
                    }
                    else
                    {
                        _logger.LogInformation("Autenticazione riuscita per: {Username})", username);
                        return true;
                    }

                 }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'autenticazione AD per l'utente {Username}", username);
                return false;
            }
        }

    }
}
