using System.Threading.Tasks;
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Services
{
    /// <summary>
    /// Servizio centralizzato per l'autenticazione utenti
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica un utente verificando le credenziali
        /// </summary>
        /// <param name="user">Utente dal database</param>
        /// <param name="password">Password fornita</param>
        /// <returns>True se autenticato, False altrimenti</returns>
        Task<bool> AuthenticateAsync(Operatori user, string password);
        
        /// <summary>
        /// Verifica se un account è bloccato per troppi tentativi
        /// </summary>
        Task<bool> IsAccountLockedAsync(string username);
        
        /// <summary>
        /// Registra tentativo fallito
        /// </summary>
        Task RecordFailedAttemptAsync(string username);
    }
}
