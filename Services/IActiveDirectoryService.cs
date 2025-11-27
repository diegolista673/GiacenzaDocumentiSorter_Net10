using System.Threading.Tasks;

namespace GiacenzaSorterRm.Services
{
    /// <summary>
    /// Servizio per autenticazione Active Directory
    /// </summary>
    public interface IActiveDirectoryService
    {
        /// <summary>
        /// Autentica utente contro Active Directory
        /// </summary>
        /// <param name="username">Username (sAMAccountName)</param>
        /// <param name="password">Password utente</param>
        /// <returns>True se credenziali valide e account attivo</returns>
        Task<bool> AuthenticateAsync(string username, string password);
        
        /// <summary>
        /// Verifica se utente esiste in AD
        /// </summary>
        Task<bool> UserExistsAsync(string username);
        
        /// <summary>
        /// Verifica se account è abilitato
        /// </summary>
        Task<bool> IsAccountEnabledAsync(string username);
    }
}
