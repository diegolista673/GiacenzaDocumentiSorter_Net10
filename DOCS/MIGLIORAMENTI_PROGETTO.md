# 📋 Specifiche Tecniche - Piano di Miglioramento Giacenza Sorter RM

**Progetto**: GiacenzaSorterRm  
**Tecnologia**: ASP.NET Core 8 Razor Pages  
**Data Analisi**: 2025  
**Versione Documento**: 1.0

---

## 📑 Indice

1. [Analisi Situazione Attuale](#analisi-situazione-attuale)
2. [Problemi Critici Identificati](#problemi-critici-identificati)
3. [Architettura Proposta](#architettura-proposta)
4. [Piano di Implementazione](#piano-di-implementazione)
5. [Specifiche Tecniche Dettagliate](#specifiche-tecniche-dettagliate)
6. [Security Best Practices](#security-best-practices)
7. [Deployment e Configurazione](#deployment-e-configurazione)

---

## 1. Analisi Situazione Attuale

### 1.1 Stack Tecnologico
- **Framework**: ASP.NET Core 8
- **Pattern**: Razor Pages
- **Database**: SQL Server (Entity Framework Core)
- **Autenticazione**: Cookie Authentication + Active Directory
- **Logging**: NLog
- **ORM**: Entity Framework Core

### 1.2 Struttura Progetto Corrente
```
GiacenzaSorterRm/
├── AppCode/                    # Utility e helper classes
├── Models/                     # Database models e view models
├── Pages/                      # Razor Pages
│   ├── Index.cshtml.cs        # Login page (PROBLEMA CRITICO)
│   ├── PagesNormalizzazione/
│   ├── PagesSorter/
│   └── ...
├── Services/                   # (DA CREARE)
├── wwwroot/                    # Static files
├── Program.cs
├── Startup.cs
└── appsettings.json
```

### 1.3 Componenti Esistenti

#### AppCode/MyConnections.cs
```csharp
public static class MyConnections
{
    // ❌ PROBLEMA: Connection string hardcoded
    public static string GiacenzaSorterRmContext { get; } = 
        "Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;User Id=UserProduzioneGed;Password=UserProduzioneGed2022!;...";
}
```

**Problemi**:
- Password in chiaro nel codice sorgente
- Difficile gestire ambienti diversi (dev/test/prod)
- Rischio di commit accidentale in repository

---

## 2. Problemi Critici Identificati

### 2.1 🔴 CRITICO - Vulnerabilità Autenticazione

#### Problema: SignIn PRIMA della Verifica Password

**File**: `Pages\Index.cshtml.cs` - Linee 66-103

```csharp
// ❌ VULNERABILITÀ CRITICA
var user = await _context.Operatoris.Include(s => s.IdCentroLavNavigation)
    .Where(x => x.Operatore == UserName).AsNoTracking().FirstOrDefaultAsync();

if (user != null)
{
    // Crea claims
    var claims = new List<Claim>{new Claim(ClaimTypes.Name, UserName)};
    // ... altri claims
    
    // ⚠️ AUTENTICAZIONE AVVIENE QUI - SENZA PASSWORD VERIFICATA!
    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

    // ❌ SOLO DOPO viene verificata la password
    if (user.Azienda == "ESTERNO")
    {
        var passwordHasher = new PasswordHasher<string>();
        var state = passwordHasher.VerifyHashedPassword(null, user.Password, Password);
        
        if (state == PasswordVerificationResult.Success)
        {
            return RedirectToPage("/Home"); // Già autenticato!
        }
    }
}
```

**Impatto**:
- **Severità**: 🔴 CRITICA
- **CVE Potenziale**: Authentication Bypass
- **CVSS Score**: 9.8 (Critical)
- **Exploit**: Chiunque conosca un username valido può autenticarsi senza password

**Proof of Concept**:
```http
POST /Index HTTP/1.1
Content-Type: application/x-www-form-urlencoded

UserName=admin&Password=qualsiasi_cosa
```
Risultato: Utente autenticato con sessione valida per 4 ore.

---

### 2.2 🔴 CRITICO - Active Directory Insicuro

**File**: `Pages\Index.cshtml.cs` - Metodo `ActiveDirectoryAuthenticate`

```csharp
public bool ActiveDirectoryAuthenticate(string username, string password)
{
    bool result = false;

    using (System.DirectoryServices.DirectoryEntry _entry = 
        new System.DirectoryServices.DirectoryEntry())
    {
        // ❌ Nessun LDAP path specificato
        _entry.Username = username;  // ❌ Password utente in chiaro
        _entry.Password = password;  // ❌ Esposta in memoria
        
        DirectorySearcher _searcher = new DirectorySearcher(_entry);
        _searcher.Filter = "(objectclass=user)";  // ❌ LDAP Injection possibile
        
        try
        {
            SearchResult _sr = _searcher.FindOne();
            if (_sr != null)
            {
                result = true;
            }
            // ❌ Searcher e SearchResult non vengono disposti
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore in active directory");
            _logger.LogError(ex.Message);  // ❌ Stack trace non loggato
        }
    }
    return result;
}
```

**Problemi Identificati**:
1. **Nessun LDAP Path**: Ricerca ambigua, performance scadenti
2. **Password in Chiaro**: Esposta in memoria, vulnerabile a dump
3. **LDAP Injection**: Filter non sanitizzato
4. **Memory Leak**: `DirectorySearcher` e `SearchResult` non disposti
5. **Logging Insufficiente**: Manca stack trace completo
6. **Nessun Service Account**: Usa credenziali utente direttamente

---

### 2.3 🟡 ALTO - Exception Handling Insicuro

```csharp
catch (Exception ex)
{
    _logger.LogError(ex.Message);
    Message = ex.Message;  // ❌ Espone dettagli tecnici all'utente
    return Page();
}
```

**Rischi**:
- Information Disclosure
- Stack traces visibili agli utenti
- Dettagli implementazione esposti

---

### 2.4 🟡 ALTO - Connection String Hardcoded

**File**: `AppCode\MyConnections.cs`

```csharp
public static string GiacenzaSorterRmContext { get; } = 
    "Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;" +
    "User Id=UserProduzioneGed;Password=UserProduzioneGed2022!;...";
```

**Problemi**:
- Password committata in repository
- Impossibile cambiare senza ricompilare
- Stessa stringa per tutti gli ambienti

---

### 2.5 🟢 MEDIO - Mancanza di Dependency Injection

- Logica di autenticazione embedded in PageModel
- Difficile testare unitariamente
- Violazione del Single Responsibility Principle
- Accoppiamento forte con DirectoryServices

---

### 2.6 🟢 MEDIO - Mancanza di Rate Limiting

- Nessuna protezione contro brute force
- Possibili attacchi DoS
- Nessun lockout dopo tentativi falliti

---

## 3. Architettura Proposta

### 3.1 Nuova Struttura a Livelli

```
┌─────────────────────────────────────────────────┐
│          Presentation Layer (Razor Pages)       │
│  - IndexModel (Login)                          │
│  - HomeModel                                    │
│  - Altri PageModels                            │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│           Service Layer (Business Logic)        │
│  - IAuthenticationService                      │
│  - IOperatoriRepository                        │
│  - IActiveDirectoryService                     │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│        Data Access Layer (EF Core)              │
│  - GiacenzaSorterRmTestContext                 │
│  - Repositories                                 │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│           External Services                     │
│  - Active Directory (LDAP)                     │
│  - SQL Server                                   │
└─────────────────────────────────────────────────┘
```

### 3.2 Dependency Injection Container

```csharp
// Startup.cs - ConfigureServices
services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<IOperatoriRepository, OperatoriRepository>();
services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();

services.Configure<ActiveDirectorySettings>(
    Configuration.GetSection("ActiveDirectory"));
```

---

## 4. Piano di Implementazione

### Fase 1: EMERGENZA - Security Hotfix (1-2 giorni)

**Obiettivo**: Risolvere vulnerabilità critica autenticazione

#### Task 1.1: Fix Flusso Autenticazione
- [ ] Spostare `SignInAsync` dopo verifica password
- [ ] Creare `AuthenticationService` base
- [ ] Aggiungere validazione input robusta
- [ ] Rimuovere `Message = ex.Message`

**File da Modificare**:
1. `Pages\Index.cshtml.cs`
2. `Services\IAuthenticationService.cs` (nuovo)
3. `Services\AuthenticationService.cs` (nuovo)

#### Task 1.2: Emergency Logging
- [ ] Rimuovere logging password
- [ ] Aggiungere structured logging
- [ ] Log tentativi falliti

---

### Fase 2: URGENTE - Sicurezza Completa (1 settimana)

#### Task 2.1: Active Directory Sicuro
- [ ] Creare `IActiveDirectoryService`
- [ ] Implementare service account
- [ ] Aggiungere LDAP path configurabile
- [ ] Implementare LDAP injection protection
- [ ] Verificare account disabilitati

#### Task 2.2: Configuration Management
- [ ] Spostare connection strings in `appsettings.json`
- [ ] Configurare User Secrets per sviluppo/test
- [ ] Configurare Variabili d'Ambiente per produzione
- [ ] Rimuovere `MyConnections.cs` hardcoded
- [ ] Verificare che secrets non siano mai committati

#### Task 2.3: Rate Limiting
- [ ] Installare `AspNetCoreRateLimit`
- [ ] Configurare limiti login (5 tentativi/minuto)
- [ ] Implementare account lockout temporaneo

---

### Fase 3: Refactoring Architettura (2-3 settimane)

#### Task 3.1: Repository Pattern
- [ ] Creare `IOperatoriRepository`
- [ ] Implementare `OperatoriRepository`
- [ ] Migrare query da PageModels

---

### Fase 4: Miglioramenti UX (1 settimana)

- [ ] Loading states durante login
- [ ] Messaggi errore user-friendly
- [ ] Validazione client-side migliorata
- [ ] Remember me functionality

---

## 5. Specifiche Tecniche Dettagliate

### 5.1 IAuthenticationService

**File**: `Services\IAuthenticationService.cs`

```csharp
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
```

---

### 5.2 AuthenticationService Implementation

**File**: `Services\AuthenticationService.cs`

```csharp
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
        private const int MAX_FAILED_ATTEMPTS = 5;
        private const int LOCKOUT_MINUTES = 15;

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
                    isAuthenticated = await AuthenticateExternalUserAsync(user, password);
                }
                else
                {
                    isAuthenticated = await _adService.AuthenticateAsync(
                        user.Operatore, 
                        password);
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

            _cache.Set(key, failedAttempts, TimeSpan.FromMinutes(LOCKOUT_MINUTES));

            _logger.LogWarning("Failed login attempt {Attempt}/{Max} for user: {Username}", 
                failedAttempts, MAX_FAILED_ATTEMPTS, username);

            if (failedAttempts >= MAX_FAILED_ATTEMPTS)
            {
                var lockoutKey = GetLockoutKey(username);
                _cache.Set(lockoutKey, true, TimeSpan.FromMinutes(LOCKOUT_MINUTES));
                
                _logger.LogWarning("Account locked for {Minutes} minutes: {Username}", 
                    LOCKOUT_MINUTES, username);
            }

            await Task.CompletedTask;
        }

        private string GetLockoutKey(string username) => 
            $"{LOCKOUT_KEY_PREFIX}{username.ToLowerInvariant()}";

        private string GetFailedAttemptsKey(string username) => 
            $"failed_attempts_{username.ToLowerInvariant()}";
    }
}
```

---

### 5.3 IActiveDirectoryService

**File**: `Services\IActiveDirectoryService.cs`

```csharp
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
```

---

### 5.4 ActiveDirectoryService Implementation

**File**: `Services\ActiveDirectoryService.cs`

```csharp
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
                    "Add 'ActiveDirectory:ServiceAccount' credentials to User Secrets or Key Vault");
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
                // Se fallisce, lancia DirectoryServicesCOMException
                object nativeObject = userEntry.NativeObject;
                
                _logger.LogInformation("Successful AD authentication: {Username}", username);
                return true;
            }
            catch (DirectoryServicesCOMException ex)
            {
                // Errore specifico AD (credenziali errate, account bloccato, ecc.)
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
                // IMPORTANTE: Rilascia risorse non managed
                userEntry?.Dispose();
                searcher?.Dispose();
                searchRoot?.Dispose();
            }
        }

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
        /// <remarks>
        /// RFC 4515 - LDAP Search Filter special characters:
        /// *, (, ), \, NUL
        /// </remarks>
        private string EscapeLdapFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return filter;

            return filter
                .Replace("\\", "\\5c")  // Backslash
                .Replace("*", "\\2a")   // Asterisk
                .Replace("(", "\\28")   // Left parenthesis
                .Replace(")", "\\29")   // Right parenthesis
                .Replace("\0", "\\00"); // NULL
        }
    }
}
```

---

### 5.5 Configuration Models

**File**: `Models\ActiveDirectorySettings.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class ActiveDirectorySettings
    {
        [Required]
        public string Domain { get; set; }

        [Required]
        public string LdapPath { get; set; }

        [Required]
        public ServiceAccountSettings ServiceAccount { get; set; }

        public bool UseServerBinding { get; set; } = true;

        [Range(5, 300)]
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class ServiceAccountSettings
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthenticationSettings
    {
        public int MaxFailedAttempts { get; set; } = 5;
        public int LockoutMinutes { get; set; } = 15;
        public bool EnableAccountLockout { get; set; } = true;
    }
}
```

---

### 5.6 IndexModel Refactored

**File**: `Pages\Index.cshtml.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using GiacenzaSorterRm.Services;
using Shyjus.BrowserDetection;

namespace GiacenzaSorterRm.Pages
{
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterRmTestContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IBrowserDetector _browserDetector;
        private readonly IAuthenticationService _authService;

        [BindProperty]
        [Required(ErrorMessage = "Username è obbligatorio")]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [BindProperty]
        [Required(ErrorMessage = "Password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [TempData]
        public string Message { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger, 
            GiacenzaSorterRmTestContext context, 
            IBrowserDetector browserDetector,
            IAuthenticationService authService)
        {
            _logger = logger;
            _context = context;
            _browserDetector = browserDetector;
            _authService = authService;
        }

        public IActionResult OnGet()
        {
            // Redirect se già autenticato
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Home");
            }

            // Check browser compatibility
            var browser = _browserDetector.Browser;
            if (browser.Name == BrowserNames.InternetExplorer)
            {
                return RedirectToPage("/IndexBrowser");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Recupera utente dal database
                var user = await _context.Operatoris
                    .Include(s => s.IdCentroLavNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Operatore == UserName);

                // ✅ VERIFICA PASSWORD PRIMA DI CREARE SESSIONE
                bool isAuthenticated = false;

                if (user != null)
                {
                    isAuthenticated = await _authService.AuthenticateAsync(user, Password);
                }
                else
                {
                    // Timing attack mitigation: esegui verifica anche se user non esiste
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }

                if (isAuthenticated && user != null)
                {
                    // SOLO DOPO verifica riuscita, crea sessione
                    await CreateAuthenticationSessionAsync(user);

                    _logger.LogInformation("Successful login for user: {Username}", UserName);
                    return RedirectToPage("/Home");
                }
                
                // Messaggio generico per evitare user enumeration
                _logger.LogWarning("Failed login attempt for username: {Username}", UserName);
                Message = "Credenziali non valide. Riprova.";
                return Page();
            }
            catch (Exception ex)
            {
                // NON esporre mai dettagli exception all'utente
                _logger.LogError(ex, "Error during login process for user: {Username}", UserName);
                Message = "Si è verificato un errore. Riprova più tardi.";
                return Page();
            }
        }

        private async Task CreateAuthenticationSessionAsync(Operatori user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Operatore),
                new Claim(ClaimTypes.Role, user.Ruolo),
                new Claim("Azienda", user.Azienda),
                new Claim("IdOperatore", user.IdOperatore.ToString()),
                new Claim("idCentro", user.IdCentroLav.ToString()),
                new Claim("Centro", user.IdCentroLavNavigation.CentroLavDesc)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4),
                IsPersistent = false,
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
```

---

## 6. Security Best Practices

### 6.1 Configuration Management

#### 🎯 Strategia di Gestione Secrets

**Obiettivo**: Rimuovere completamente le password hardcoded dal codice sorgente e gestire le configurazioni sensibili in modo sicuro per ogni ambiente.

**Approccio**:
- ✅ **Development/Test**: User Secrets (locale, non committato)
- ✅ **Produzione**: Variabili d'Ambiente (configurate nel server/IIS)

---

#### ⚙️ STEP 1: Eliminare Hardcoded Connection Strings

**File da Rimuovere**: `AppCode\MyConnections.cs`

```bash
# Rimuovi il file con password hardcoded
git rm AppCode\MyConnections.cs
git commit -m "Remove hardcoded connection strings - Security Fix"
```

**Verifica Utilizzo nel Codice**:
```bash
# Cerca riferimenti a MyConnections
findstr /s /i "MyConnections" *.cs
```

Se trovi riferimenti, sostituiscili con dependency injection del `DbContext`.

---

#### 🔧 STEP 2: Configurare User Secrets (Development/Test)

**Setup User Secrets**:
```bash
# Naviga nella directory del progetto
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm

# Inizializza User Secrets (aggiunge UserSecretsId al .csproj)
dotnet user-secrets init

# Aggiungi Connection String per ambiente di test
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\\SQLEXPRESS;Database=GIACENZA_SORTER_RM_TEST;Trusted_Connection=True;TrustServerCertificate=True;"

# Aggiungi credenziali Active Directory Service Account
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Username" "POSTEL\\svc_giacenza_test"
dotline user-secrets set "ActiveDirectory:ServiceAccount:Password" "TestPassword123!"

# OPZIONALE: Se hai bisogno di override di altre configurazioni
dotnet user-secrets set "ActiveDirectory:LdapPath" "LDAP://test-dc.postel.it"
dotnet user-secrets set "Authentication:MaxFailedAttempts" "3"
```

**Verifica Secrets Configurati**:
```bash
# Lista tutti i secrets
dotnet user-secrets list
```

**Output Atteso**:
```
ConnectionStrings:DefaultConnection = Server=.\SQLEXPRESS;Database=GIACENZA_SORTER_RM_TEST;...
ActiveDirectory:ServiceAccount:Username = POSTEL\svc_giacenza_test
ActiveDirectory:ServiceAccount:Password = TestPassword123!
```

**Dove Vengono Salvati i Secrets?**
```
Windows: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
Linux/Mac: ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
```

**File Non Committato**: I secrets sono salvati **fuori** dalla directory del progetto e **non verranno mai** committati in Git.

---

#### 🔧 STEP 3: Configurare Variabili d'Ambiente (Produzione)

##### Windows Server con IIS

**1. Apri IIS Manager**

**2. Naviga all'Application Pool**:
```
Application Pools → Seleziona il tuo pool → Advanced Settings
```

**3. Configura Environment Variables**:
```
Configuration Editor → Section: system.applicationHost/applicationPools
→ Seleziona il tuo pool → environmentVariables → Add

Variabile 1:
  Name: ConnectionStrings__DefaultConnection
  Value: Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;User Id=UserProduzioneGed;Password=UserProduzioneGed2022!;TrustServerCertificate=True;

Variabile 2:
  Name: ActiveDirectory__ServiceAccount__Username
  Value: POSTEL\svc_giacenza

Variabile 3:
  Name: ActiveDirectory__ServiceAccount__Password
  Value: ComplexServiceAccountPassword!

Variabile 4:
  Name: ASPNETCORE_ENVIRONMENT
  Value: Production
```

**Nota Sintassi**: Usa `__` (doppio underscore) per rappresentare `:` nella gerarchia JSON.

**4. Restart Application Pool**:
```powershell
Import-Module WebAdministration
Restart-WebAppPool -Name "GiacenzaSorterRmAppPool"
```

**Alternativa: Configurazione via web.config**

Se preferisci configurare le variabili d'ambiente direttamente nel file `web.config` del sito:

```xml
<configuration>
  <system.webServer>
    <aspNetCore processPath="dotnet" 
                 arguments=".\GiacenzaSorterRm.dll"
                 stdoutLogEnabled="true" 
                 stdoutLogFile=".\logs\stdout" 
                 hostingModel="InProcess">
      <environmentVariables>
        <environmentVariable name="ConnectionStrings__DefaultConnection" 
                            value="Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;User Id=UserProduzioneGed;Password=UserProduzioneGed2022!;TrustServerCertificate=True;" />
        <environmentVariable name="ActiveDirectory__ServiceAccount__Username" 
                            value="POSTEL\svc_giacenza" />
        <environmentVariable name="ActiveDirectory__ServiceAccount__Password" 
                            value="ComplexServiceAccountPassword!" />
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" 
                            value="Production" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>
```

**⚠️ Nota Sicurezza**: Il file `web.config` con secrets **NON deve essere committato** in Git. Aggiungi al `.gitignore`:
```gitignore
# Web.config con secrets (produzione)
web.config
```

---

## 7. Deployment e Configurazione

### 7.1 Requisiti di Sistema

- **.NET Runtime**: 8.x
- **Database**: SQL Server 2019 o superiore
- **Sistema Operativo**: Windows Server 2019 o superiore
- **IIS**: 10.0 con supporto per ASP.NET Core

---

### 7.2 Configurazione Ambiente di Produzione

1. **Creare un nuovo sito in IIS**:
   - Nome: `GiacenzaSorterRm`
   - Percorso fisico: `C:\inetpub\wwwroot\GiacenzaSorterRm`

2. **Configurare Application Pool**:
   - Nome: `GiacenzaSorterRmAppPool`
   - Versione .NET: **No Managed Code**
   - Autenticazione: **ApplicationPoolIdentity**

3. **Impostare variabili d'ambiente** nel file `C:\inetpub\wwwroot\GiacenzaSorterRm\web.config`:
```xml
<configuration>
  <system.webServer>
    <aspNetCore processPath="dotnet" 
                 arguments=".\GiacenzaSorterRm.dll"
                 stdoutLogEnabled="true" 
                 stdoutLogFile=".\logs\stdout" 
                 hostingModel="InProcess">
      <environmentVariables>
        <environmentVariable name="ConnectionStrings__DefaultConnection" 
                            value="Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;User Id=UserProduzioneGed;Password=UserProduzioneGed2022!;TrustServerCertificate=True;" />
        <environmentVariable name="ActiveDirectory__ServiceAccount__Username" 
                            value="POSTEL\svc_giacenza" />
        <environmentVariable name="ActiveDirectory__ServiceAccount__Password" 
                            value="ComplexServiceAccountPassword!" />
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" 
                            value="Production" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>
```

4. **Configurare SQL Server**:
   - Creare il database `GIACENZA_SORTER_RM`
   - Eseguire gli script di migrazione da `db\migrations`

5. **Configurare Active Directory**:
   - Creare un **service account** per l'applicazione
   - Assegnare permessi di lettura sugli oggetti utente

---

### 7.3 CI/CD Pipeline (Esempio)

**azure-pipelines.yml** (o GitHub Actions equivalente):
```yaml
trigger:
  - main

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  dotnetSdkVersion: '8.x'

stages:
- stage: Build
  jobs:
  - job: Build
    steps:
    - task: UseDotNet@2
      inputs:
        version: $(dotnetSdkVersion)
        
    - task: DotNetCoreCLI@2
      displayName: 'Restore'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
        
    - task: DotNetCoreCLI@2
      displayName: 'Build'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration)'
        
    - task: DotNetCoreCLI@2
      displayName: 'Run Tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage"'
        
    - task: DotNetCoreCLI@2
      displayName: 'Publish'
      inputs:
        command: 'publish'
        publishWebProjects: true
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
        
    - task: PublishBuildArtifacts@1
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'

- stage: Deploy
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployToProduction
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: IISWebAppManagementOnMachineGroup@0
            displayName: 'Stop IIS Application Pool'
            inputs:
              IISDeploymentType: 'IISApplicationPool'
              ActionIISApplicationPool: 'StopAppPool'
              StartStopRecycleAppPoolName: 'GiacenzaSorterRmAppPool'
              
          - task: IISWebAppDeploymentOnMachineGroup@0
            displayName: 'Deploy to IIS'
            inputs:
              WebSiteName: 'GiacenzaSorterRm'
              Package: '$(Pipeline.Workspace)/drop/**/*.zip'
              RemoveAdditionalFilesFlag: true
              TakeAppOfflineFlag: true
              
          - task: IISWebAppManagementOnMachineGroup@0
            displayName: 'Start IIS Application Pool'
            inputs:
              IISDeploymentType: 'IISApplicationPool'
              ActionIISApplicationPool: 'StartAppPool'
              StartStopRecycleAppPoolName: 'GiacenzaSorterRmAppPool'
```

**Nota**: Le variabili d'ambiente sensibili vengono configurate direttamente sul server IIS, **non** nella pipeline.

---
