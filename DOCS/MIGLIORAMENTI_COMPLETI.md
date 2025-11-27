# ?? Piano Completo di Miglioramenti - GiacenzaSorterRm

**Progetto**: GiacenzaSorterRm  
**Framework**: ASP.NET Core 10.0 Razor Pages  
**Data Documento**: 2025  
**Versione**: 2.0

---

## ?? Indice

1. [Miglioramenti Sicurezza](#1-miglioramenti-sicurezza)
2. [Modernizzazione JavaScript - Rimozione jQuery Ajax](#2-modernizzazione-javascript---rimozione-jquery-ajax)
3. [Miglioramenti Architetturali](#3-miglioramenti-architetturali)
4. [Miglioramenti Performance](#4-miglioramenti-performance)
5. [Miglioramenti UX/UI](#5-miglioramenti-uxui)
6. [Miglioramenti DevOps e Deployment](#6-miglioramenti-devops-e-deployment)
7. [Miglioramenti Testing](#7-miglioramenti-testing)
8. [Miglioramenti Logging e Monitoring](#8-miglioramenti-logging-e-monitoring)
9. [Miglioramenti Database](#9-miglioramenti-database)
10. [Documentazione e Manutenibilità](#10-documentazione-e-manutenibilità)

---

## 1. Miglioramenti Sicurezza

### ?? PRIORITÀ ALTA

#### 1.1 Autenticazione e Autorizzazione
- [x] ? **Correzione Authentication Bypass** - COMPLETATO
  - Verifica password PRIMA di SignIn
  - File: `Pages/Index.cshtml.cs`

- [x] ? **Implementazione Servizi di Autenticazione** - COMPLETATO
  - `IAuthenticationService` e `AuthenticationService`
  - Account lockout automatico dopo 5 tentativi
  - File: `Services/AuthenticationService.cs`

- [ ] ?? **Implementare Two-Factor Authentication (2FA)**
  - Supporto TOTP (Google Authenticator, Microsoft Authenticator)
  - Email/SMS backup codes
  - Package: `AspNetCore.Identity.UI`

- [ ] ?? **Implementare Password Policy Robusta**
  ```csharp
  // In Startup.cs ConfigureServices
  services.Configure<IdentityOptions>(options =>
  {
      options.Password.RequireDigit = true;
      options.Password.RequiredLength = 12;
      options.Password.RequireNonAlphanumeric = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireLowercase = true;
      options.Password.RequiredUniqueChars = 4;
  });
  ```

- [ ] ?? **Implementare Session Timeout Configurabile**
  ```csharp
  services.AddAuthentication().AddCookie(options =>
  {
      options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
      options.SlidingExpiration = true;
  });
  ```

- [ ] ?? **Aggiungere Claims-Based Authorization Granulare**
  - Creare policy per singole funzionalità
  - Implementare `IAuthorizationHandler` custom
  - File: `Authorization/PermissionHandler.cs` (DA CREARE)

#### 1.2 Connection String e Secrets Management
- [x] ? **Rimozione Connection String Hardcoded** - COMPLETATO
  - User Secrets per Development
  - Environment Variables per Production
  - File: `appsettings.json`, `Program.cs`

- [ ] ?? **Integrazione Azure Key Vault**
  ```csharp
  // In Program.cs
  .ConfigureAppConfiguration((context, config) =>
  {
      if (context.HostingEnvironment.IsProduction())
      {
          var builtConfig = config.Build();
          var keyVaultEndpoint = builtConfig["KeyVault:Endpoint"];
          config.AddAzureKeyVault(
              new Uri(keyVaultEndpoint),
              new DefaultAzureCredential());
      }
  })
  ```
  - Package: `Azure.Extensions.AspNetCore.Configuration.Secrets`
  - Benefici: Rotation automatica password, audit trail

#### 1.3 Active Directory Security
- [x] ? **LDAP Injection Protection** - COMPLETATO
  - Escape caratteri speciali
  - File: `Services/ActiveDirectoryService.cs`

- [ ] ?? **Implementare LDAPS (LDAP over SSL)**
  ```csharp
  LdapPath = "LDAPS://postel.it:636"
  ```
  - Configurare certificato SSL per Domain Controller
  - Aggiornare `appsettings.json`

- [ ] ?? **Implementare Account Service con Least Privilege**
  - Creare account AD dedicato con permessi minimi
  - Solo lettura su OU utenti
  - Documentare permessi necessari

#### 1.4 Cookie Security
- [ ] ?? **Configurare Cookie Security Flags**
  ```csharp
  services.AddAuthentication().AddCookie(options =>
  {
      options.Cookie.HttpOnly = true;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      options.Cookie.SameSite = SameSiteMode.Strict;
      options.Cookie.Name = "__Host-GiacenzaSorter"; // __Host- prefix per security
  });
  ```

- [ ] ?? **Implementare CSRF Token per AJAX Calls**
  - Aggiungere antiforgery token header
  - Validare su tutte le POST requests

#### 1.5 HTTPS Enforcement
- [ ] ?? **Forzare HTTPS in Produzione**
  ```csharp
  // In Startup.cs Configure
  if (!env.IsDevelopment())
  {
      app.UseHsts();
      app.UseHttpsRedirection();
  }
  ```

- [ ] ?? **Configurare HSTS Header**
  ```csharp
  services.AddHsts(options =>
  {
      options.MaxAge = TimeSpan.FromDays(365);
      options.IncludeSubDomains = true;
      options.Preload = true;
  });
  ```

#### 1.6 SQL Injection Protection
- [x] ? **Uso Entity Framework (Parametrizzato)** - GIÀ IMPLEMENTATO
  - Tutte le query usano EF Core
  - Protezione automatica

- [ ] ?? **Code Review per Raw SQL**
  - Verificare uso di `FromSqlRaw` / `ExecuteSqlRaw`
  - Assicurare parametrizzazione
  - File: Tutti i PageModel con query database

#### 1.7 Rate Limiting Avanzato
- [ ] ?? **Implementare AspNetCoreRateLimit**
  ```bash
  dotnet add package AspNetCoreRateLimit
  ```
  ```csharp
  // In Startup.cs
  services.AddMemoryCache();
  services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
  services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
  services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
  services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
  
  // In Configure
  app.UseIpRateLimiting();
  ```
  - Configurare limiti per endpoint
  - Whitelist IP interni
  - File: `appsettings.json` sezione `IpRateLimiting`

#### 1.8 Security Headers
- [ ] ?? **Implementare Security Headers Middleware**
  ```bash
  dotnet add package NWebsec.AspNetCore.Middleware
  ```
  ```csharp
  // In Startup.cs Configure
  app.UseXContentTypeOptions();
  app.UseReferrerPolicy(opts => opts.NoReferrer());
  app.UseXXssProtection(options => options.EnabledWithBlockMode());
  app.UseXfo(options => options.Deny());
  app.UseCsp(opts => opts
      .DefaultSources(s => s.Self())
      .ScriptSources(s => s.Self())
      .StyleSources(s => s.Self())
  );
  ```

#### 1.9 Input Validation
- [ ] ?? **Aggiungere FluentValidation**
  ```bash
  dotnet add package FluentValidation.AspNetCore
  ```
  - Creare validators per tutti i DTOs
  - Validazione server-side robusta
  - File: `Validators/` (DA CREARE)

- [ ] ?? **Sanitizzazione Input Utente**
  - Implementare HtmlSanitizer per input rich text
  - Whitelist caratteri permessi
  - Package: `HtmlSanitizer`

#### 1.10 Audit Logging
- [ ] ?? **Implementare Audit Trail Completo**
  ```csharp
  public class AuditLog
  {
      public int Id { get; set; }
      public string UserId { get; set; }
      public string Action { get; set; }
      public string EntityType { get; set; }
      public string EntityId { get; set; }
      public DateTime Timestamp { get; set; }
      public string IpAddress { get; set; }
      public string Changes { get; set; } // JSON
  }
  ```
  - Tracciare tutte le operazioni CRUD
  - Tracciare login/logout
  - Retention policy 1 anno minimo

---

## 2. Modernizzazione JavaScript - Rimozione jQuery Ajax

### ?? PRIORITÀ ALTA - Sostituire jQuery.ajax con Fetch API

#### 2.1 Analisi Situazione Attuale

**File con jQuery Unobtrusive Ajax Identificati:**
- `Pages/PagesNormalizzato/Index.cshtml`
- `Pages/PagesMacero/Index.cshtml`
- `Pages/TipologiaNormalizzazione/Index.cshtml`
- Altri file `.cshtml` con `data-ajax="true"`

**Problemi:**
- ? Dipendenza da `jquery.unobtrusive-ajax.js` (libreria obsoleta)
- ? Syntax non standard
- ? Performance subottimale
- ? Difficile debugging
- ? Non compatibile con moderne SPA frameworks

#### 2.2 Piano di Migrazione a Fetch API

##### Step 1: Creare Utility Module JavaScript
**File:** `wwwroot/js/fetch-helpers.js` (DA CREARE)
```javascript
// Fetch API Helpers per GiacenzaSorterRm
export const FetchHelpers = {
    /**
     * Esegue una POST request con antiforgery token
     * @param {string} url - URL endpoint
     * @param {FormData|Object} data - Dati da inviare
     * @param {Function} onSuccess - Callback successo
     * @param {Function} onError - Callback errore
     */
    async postWithAntiforgery(url, data, onSuccess, onError) {
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const headers = {
                'RequestVerificationToken': token
            };

            let body;
            if (data instanceof FormData) {
                body = data;
            } else {
                headers['Content-Type'] = 'application/json';
                body = JSON.stringify(data);
            }

            const response = await fetch(url, {
                method: 'POST',
                headers: headers,
                body: body,
                credentials: 'same-origin'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.text(); // Razor Pages ritorna HTML
            if (onSuccess) onSuccess(result);
            return result;

        } catch (error) {
            console.error('Fetch error:', error);
            if (onError) onError(error);
            throw error;
        }
    },

    /**
     * Esegue una GET request
     */
    async get(url, onSuccess, onError) {
        try {
            const response = await fetch(url, {
                method: 'GET',
                credentials: 'same-origin'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.text();
            if (onSuccess) onSuccess(result);
            return result;

        } catch (error) {
            console.error('Fetch error:', error);
            if (onError) onError(error);
            throw error;
        }
    },

    /**
     * Mostra/Nascondi spinner
     */
    showSpinner(elementId) {
        const spinner = document.getElementById(elementId);
        if (spinner) spinner.style.display = 'block';
    },

    hideSpinner(elementId) {
        const spinner = document.getElementById(elementId);
        if (spinner) spinner.style.display = 'none';
    }
};
```

##### Step 2: Migrare PagesNormalizzato/Index.cshtml

**PRIMA (jQuery Unobtrusive Ajax):**
```razor
<form class="form-inline justify-content-center" method="post"
      data-ajax="true"
      data-ajax-method="post"
      data-ajax-update="#dvReport"
      data-ajax-mode="replace"
      data-ajax-url="?handler=Report"
      data-ajax-success="OnSuccessRequest">
```

**DOPO (Fetch API):**
```razor
<form class="form-inline justify-content-center" method="post" id="reportForm">
    <!-- form fields... -->
    <button type="submit" class="btn btn-primary">Report</button>
</form>

@section scripts {
    <script type="module">
        import { FetchHelpers } from '/js/fetch-helpers.js';

        document.getElementById('reportForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const form = e.target;
            const formData = new FormData(form);
            const url = '?handler=Report';
            
            // Mostra spinner
            document.getElementById('divProcessing').style.display = 'block';
            
            try {
                const result = await FetchHelpers.postWithAntiforgery(
                    url, 
                    formData,
                    (html) => {
                        // Success callback
                        document.getElementById('dvReport').innerHTML = html;
                        document.getElementById('divProcessing').style.display = 'none';
                    },
                    (error) => {
                        // Error callback
                        alert('Errore durante il caricamento del report');
                        document.getElementById('divProcessing').style.display = 'none';
                    }
                );
            } catch (error) {
                console.error('Submit error:', error);
            }
        });
    </script>
}
```

##### Step 3: Migrare PagesMacero/Index.cshtml
- File: `Pages/PagesMacero/Index.cshtml`
- Stesso pattern di sopra
- Rimuovere dipendenza `jquery.unobtrusive-ajax.js`

##### Step 4: Aggiornare _Layout.cshtml
**Rimuovere:**
```html
<script src="~/lib/jquery-unobtrusive-ajax/jquery.unobtrusive-ajax.js"></script>
```

**Aggiungere (se necessario jQuery per altri usi):**
```html
<!-- Mantieni jQuery solo per Bootstrap e DataTables -->
<script src="~/lib/jquery/dist/jquery.min.js"></script>
```

##### Step 5: Package.json per Gestione Dipendenze JavaScript
**File:** `package.json` (DA CREARE)
```json
{
  "name": "giacenza-sorter-rm",
  "version": "1.0.0",
  "description": "Frontend dependencies for GiacenzaSorterRm",
  "scripts": {
    "build": "webpack --mode production",
    "dev": "webpack --mode development --watch"
  },
  "devDependencies": {
    "webpack": "^5.89.0",
    "webpack-cli": "^5.1.4"
  }
}
```

#### 2.3 Benefici Fetch API vs jQuery Ajax

| Aspetto | jQuery Ajax | Fetch API |
|---------|-------------|-----------|
| **Size** | ~85KB (jQuery completo) | ~0KB (nativo browser) |
| **Performance** | Overhead jQuery | Nativo, più veloce |
| **Promises** | Callbacks/Promises custom | Promise native ES6 |
| **Async/Await** | Non supportato nativamente | Supporto completo |
| **Maintenance** | Dipendenza esterna | Standard web |
| **TypeScript** | Tipizzazione limitata | Tipizzazione completa |

#### 2.4 Compatibilità Browser
- ? Chrome 42+
- ? Firefox 39+
- ? Safari 10.1+
- ? Edge 14+
- ?? IE11: Polyfill necessario (non supportato da .NET 10)

#### 2.5 Checklist Migrazione

- [ ] **Creare `fetch-helpers.js`**
- [ ] **Identificare tutti i form con `data-ajax="true"`**
  - Usare ricerca globale: `data-ajax="true"`
- [ ] **Migrare form uno per uno**
  - Priorità: form usati frequentemente
- [ ] **Testare ogni migrazione**
  - Verificare submit corretto
  - Verificare gestione errori
  - Verificare spinner/loading state
- [ ] **Rimuovere `jquery.unobtrusive-ajax.js`**
- [ ] **Aggiornare documentazione**

---

## 3. Miglioramenti Architetturali

### ?? PRIORITÀ MEDIA

#### 3.1 Repository Pattern
- [ ] ??? **Implementare Repository Pattern per Database Access**
  ```csharp
  // Interfaccia
  public interface IOperatoriRepository
  {
      Task<Operatori> GetByUsernameAsync(string username);
      Task<IEnumerable<Operatori>> GetAllAsync();
      Task AddAsync(Operatori operatore);
      Task UpdateAsync(Operatori operatore);
      Task DeleteAsync(int id);
  }
  
  // Implementazione
  public class OperatoriRepository : IOperatoriRepository
  {
      private readonly GiacenzaSorterRmTestContext _context;
      
      public OperatoriRepository(GiacenzaSorterRmTestContext context)
      {
          _context = context;
      }
      
      // Implementazione metodi...
  }
  ```
  - File: `Repositories/` (DA CREARE)
  - Benefici: Testabilità, separazione concerns, riusabilità

- [ ] ??? **Creare Repository per ogni Entity**
  - `IScatoleRepository` / `ScatoleRepository`
  - `IBancaliRepository` / `BancaliRepository`
  - `ICommesseRepository` / `CommesseRepository`

#### 3.2 Service Layer
- [ ] ??? **Creare Business Logic Services**
  ```csharp
  public interface IGiacenzaService
  {
      Task<GiacenzaView> GetGiacenzaByCodiceAsync(string codice);
      Task SpostaGiacenzaAsync(string from, string to, int operatoreId);
      Task<IEnumerable<GiacenzaView>> GetGiacenzeByCommessaAsync(int commessaId);
  }
  ```
  - File: `Services/IGiacenzaService.cs` (DA CREARE)
  - Separare business logic da PageModels

#### 3.3 DTOs (Data Transfer Objects)
- [ ] ??? **Creare DTOs per API Responses**
  ```csharp
  public class ScatolaDto
  {
      public int Id { get; set; }
      public string CodiceScatola { get; set; }
      public DateTime DataInserimento { get; set; }
      // Solo campi necessari per UI
  }
  ```
  - Non esporre entità database direttamente
  - File: `Models/DTOs/` (DA CREARE)

#### 3.4 AutoMapper
- [ ] ??? **Implementare AutoMapper per Entity-DTO Mapping**
  ```bash
  dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
  ```
  ```csharp
  // In Startup.cs
  services.AddAutoMapper(typeof(Startup));
  
  // Profiles
  public class ScatolaProfile : Profile
  {
      public ScatolaProfile()
      {
          CreateMap<Scatole, ScatolaDto>();
          CreateMap<ScatolaDto, Scatole>();
      }
  }
  ```

#### 3.5 CQRS Pattern (Command Query Responsibility Segregation)
- [ ] ??? **Separare Read e Write Operations**
  ```csharp
  // Commands (Write)
  public class CreateScatolaCommand : IRequest<int>
  {
      public string CodiceScatola { get; set; }
      public int OperatoreId { get; set; }
  }
  
  public class CreateScatolaCommandHandler : IRequestHandler<CreateScatolaCommand, int>
  {
      // Handler logic...
  }
  
  // Queries (Read)
  public class GetScatolaByIdQuery : IRequest<ScatolaDto>
  {
      public int Id { get; set; }
  }
  ```
  - Package: `MediatR`
  - Benefici: Scalabilità, testabilità, separazione concerns

#### 3.6 Dependency Injection Best Practices
- [x] ? **Servizi di Autenticazione registrati** - COMPLETATO

- [ ] ??? **Registrare tutti i Repositories e Services**
  ```csharp
  // In Startup.cs ConfigureServices
  services.AddScoped<IOperatoriRepository, OperatoriRepository>();
  services.AddScoped<IScatoleRepository, ScatoleRepository>();
  services.AddScoped<IGiacenzaService, GiacenzaService>();
  ```

- [ ] ??? **Usare Service Lifetimes corretti**
  - `AddScoped`: Per servizi con stato per request (Repositories, DB Context)
  - `AddSingleton`: Per servizi stateless (Configuration, Caching)
  - `AddTransient`: Per servizi lightweight (Validators, Mappers)

#### 3.7 Configuration Management
- [ ] ??? **Creare Strongly-Typed Configuration Classes**
  ```csharp
  public class SmtpSettings
  {
      public string Host { get; set; }
      public int Port { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }
  }
  
  // In Startup.cs
  services.Configure<SmtpSettings>(Configuration.GetSection("Smtp"));
  ```

#### 3.8 Exception Handling Middleware
- [ ] ??? **Creare Global Exception Handler**
  ```csharp
  public class GlobalExceptionHandlerMiddleware
  {
      private readonly RequestDelegate _next;
      private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

      public async Task InvokeAsync(HttpContext context)
      {
          try
          {
              await _next(context);
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Unhandled exception");
              await HandleExceptionAsync(context, ex);
          }
      }

      private static Task HandleExceptionAsync(HttpContext context, Exception exception)
      {
          context.Response.StatusCode = 500;
          return context.Response.WriteAsJsonAsync(new
          {
              error = "Si è verificato un errore. Contattare il supporto."
          });
      }
  }
  ```

---

## 4. Miglioramenti Performance

### ?? PRIORITÀ MEDIA

#### 4.1 Database Query Optimization
- [ ] ? **Implementare Query Lazy Loading Selettivo**
  ```csharp
  // Disabilitare lazy loading globale
  services.AddDbContext<GiacenzaSorterRmTestContext>(options =>
  {
      options.UseSqlServer(connectionString);
      options.UseLazyLoadingProxies(false); // Disable lazy loading
  });
  
  // Usare Include() esplicito quando necessario
  var scatole = await _context.Scatole
      .Include(s => s.Commessa)
      .Include(s => s.Operatore)
      .ToListAsync();
  ```

- [ ] ? **Aggiungere Paginazione per Liste Grandi**
  ```csharp
  public async Task<PagedResult<ScatolaDto>> GetScatolePagedAsync(int pageNumber, int pageSize)
  {
      var totalCount = await _context.Scatole.CountAsync();
      
      var scatole = await _context.Scatole
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();
      
      return new PagedResult<ScatolaDto>
      {
          Items = _mapper.Map<List<ScatolaDto>>(scatole),
          TotalCount = totalCount,
          PageNumber = pageNumber,
          PageSize = pageSize
      };
  }
  ```

- [ ] ? **Creare Indici Database per Query Frequenti**
  ```csharp
  // In DbContext OnModelCreating
  modelBuilder.Entity<Scatole>()
      .HasIndex(s => s.CodiceScatola)
      .IsUnique();
  
  modelBuilder.Entity<Bancali>()
      .HasIndex(b => new { b.IdCommessa, b.DataInserimento });
  ```

- [ ] ? **Implementare AsNoTracking per Query Read-Only**
  ```csharp
  var scatole = await _context.Scatole
      .AsNoTracking() // Non traccia entità in memoria
      .Where(s => s.IdCommessa == commessaId)
      .ToListAsync();
  ```

#### 4.2 Caching Strategy
- [ ] ? **Implementare Response Caching**
  ```csharp
  // In Startup.cs
  services.AddResponseCaching();
  
  // In Configure
  app.UseResponseCaching();
  
  // In PageModel
  [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
  public async Task<IActionResult> OnGetAsync()
  {
      // ...
  }
  ```

- [ ] ? **Implementare Distributed Cache per Scalabilità**
  ```bash
  dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
  ```
  ```csharp
  services.AddStackExchangeRedisCache(options =>
  {
      options.Configuration = Configuration.GetConnectionString("Redis");
      options.InstanceName = "GiacenzaSorter_";
  });
  ```

- [ ] ? **Cache Dati Statici (Tipologie, Contenitori, ecc.)**
  ```csharp
  public class TipologieService
  {
      private readonly IMemoryCache _cache;
      
      public async Task<List<Tipologie>> GetAllAsync()
      {
          return await _cache.GetOrCreateAsync("tipologie", async entry =>
          {
              entry.SlidingExpiration = TimeSpan.FromHours(24);
              return await _context.Tipologies.ToListAsync();
          });
      }
  }
  ```

#### 4.3 Async/Await Optimization
- [ ] ? **Code Review per Async Operations**
  - Verificare che tutte le operazioni I/O siano async
  - Rimuovere `.Result` / `.Wait()` (causano deadlock)
  - Usare `ConfigureAwait(false)` dove appropriato

#### 4.4 JavaScript Optimization
- [ ] ? **Minificazione e Bundling**
  ```bash
  dotnet add package BuildBundlerMinifier
  ```
  - File: `bundleconfig.json` (DA CREARE)
  ```json
  [
    {
      "outputFileName": "wwwroot/css/site.min.css",
      "inputFiles": [
        "wwwroot/css/site.css"
      ]
    },
    {
      "outputFileName": "wwwroot/js/site.min.js",
      "inputFiles": [
        "wwwroot/js/site.js",
        "wwwroot/js/fetch-helpers.js"
      ]
    }
  ]
  ```

- [ ] ? **Lazy Loading JavaScript Libraries**
  ```html
  <!-- Carica DataTables solo se necessario -->
  @if (ViewData["UseDataTables"] != null)
  {
      <script src="~/lib/datatables/datatables.min.js"></script>
  }
  ```

#### 4.5 Image Optimization
- [ ] ? **Ottimizzare Immagini Statiche**
  - Convertire PNG/JPG in WebP
  - Implementare lazy loading immagini
  - Usare CDN per static assets

#### 4.6 Compression
- [ ] ? **Abilitare Response Compression**
  ```csharp
  // In Startup.cs
  services.AddResponseCompression(options =>
  {
      options.EnableForHttps = true;
      options.Providers.Add<BrotliCompressionProvider>();
      options.Providers.Add<GzipCompressionProvider>();
  });
  
  // In Configure (prima di UseStaticFiles)
  app.UseResponseCompression();
  ```

---

## 5. Miglioramenti UX/UI

### ?? PRIORITÀ BASSA

#### 5.1 UI Framework Modernization
- [ ] ?? **Aggiornare Bootstrap a Versione Recente**
  - Attuale: Bootstrap (versione da verificare)
  - Target: Bootstrap 5.3
  - Benefici: Migliore responsive, dark mode nativo

- [ ] ?? **Implementare Design System Consistente**
  - Creare file `design-tokens.css`
  ```css
  :root {
      --color-primary: #0066cc;
      --color-secondary: #6c757d;
      --color-success: #28a745;
      --color-danger: #dc3545;
      --spacing-sm: 8px;
      --spacing-md: 16px;
      --spacing-lg: 24px;
  }
  ```

#### 5.2 DataTables Enhancement
- [ ] ?? **Ottimizzare DataTables Configuration**
  ```javascript
  $('#tableProduzione').DataTable({
      responsive: true,
      language: {
          url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/it-IT.json'
      },
      pageLength: 25,
      stateSave: true, // Salva stato paginazione/filtri
      order: [[0, 'desc']], // Ordina per prima colonna
      columnDefs: [
          { orderable: false, targets: -1 } // Disabilita ordinamento su colonna azioni
      ]
  });
  ```

#### 5.3 Loading States
- [ ] ?? **Implementare Skeleton Screens**
  - Miglior UX rispetto a spinner generico
  - Mostrare struttura contenuto durante caricamento

- [ ] ?? **Progress Bar per Upload File**
  ```javascript
  const xhr = new XMLHttpRequest();
  xhr.upload.addEventListener('progress', (e) => {
      if (e.lengthComputable) {
          const percentComplete = (e.loaded / e.total) * 100;
          updateProgressBar(percentComplete);
      }
  });
  ```

#### 5.4 Validazione Client-Side
- [ ] ?? **Migliorare Feedback Validazione**
  ```javascript
  // Real-time validation
  document.getElementById('codiceScatola').addEventListener('blur', async (e) => {
      const value = e.target.value;
      const isValid = await validateCodiceScatola(value);
      
      if (!isValid) {
          e.target.classList.add('is-invalid');
          showErrorMessage('Codice scatola già esistente');
      } else {
          e.target.classList.remove('is-invalid');
          e.target.classList.add('is-valid');
      }
  });
  ```

#### 5.5 Toast Notifications
- [ ] ?? **Sostituire Alert con Toast**
  ```bash
  npm install toastr
  ```
  ```javascript
  // Invece di alert()
  toastr.success('Operazione completata con successo!');
  toastr.error('Si è verificato un errore');
  toastr.warning('Attenzione: dati non salvati');
  ```

#### 5.6 Accessibility (A11y)
- [ ] ?? **Migliorare Accessibilità WCAG 2.1 Level AA**
  - Aggiungere `aria-labels` a icone e pulsanti
  - Supporto navigazione da tastiera
  - Contrasto colori sufficiente
  - Screen reader friendly

#### 5.7 Dark Mode
- [ ] ?? **Implementare Dark Mode**
  ```css
  @media (prefers-color-scheme: dark) {
      :root {
          --bg-color: #1a1a1a;
          --text-color: #ffffff;
      }
  }
  ```

#### 5.8 Print Styles
- [ ] ?? **Ottimizzare CSS per Stampa Report**
  ```css
  @media print {
      .navbar, .sidebar, .btn { display: none; }
      table { page-break-inside: avoid; }
  }
  ```

---

## 6. Miglioramenti DevOps e Deployment

### ?? PRIORITÀ MEDIA

#### 6.1 CI/CD Pipeline
- [ ] ?? **Implementare GitHub Actions CI/CD**
  
  **File:** `.github/workflows/ci-cd.yml` (DA CREARE)
  ```yaml
  name: CI/CD Pipeline
  
  on:
    push:
      branches: [ master, develop ]
    pull_request:
      branches: [ master ]
  
  jobs:
    build-and-test:
      runs-on: windows-latest
      
      steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET 10
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --configuration Release --no-restore
      
      - name: Run tests
        run: dotnet test --no-build --verbosity normal
      
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      
      - name: Upload artifact
        uses: actions/upload-artifact@v3
        with:
          name: giacenza-sorter-app
          path: ./publish
    
    deploy-to-iis:
      needs: build-and-test
      runs-on: windows-latest
      if: github.ref == 'refs/heads/master'
      
      steps:
      - name: Download artifact
        uses: actions/download-artifact@v3
        with:
          name: giacenza-sorter-app
      
      - name: Deploy to IIS
        run: |
          # Script PowerShell per deploy IIS
          # Ferma app pool, copia file, riavvia app pool
  ```

#### 6.2 Docker Containerization
- [ ] ?? **Creare Dockerfile**
  
  **File:** `Dockerfile` (DA CREARE)
  ```dockerfile
  FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
  WORKDIR /app
  EXPOSE 80
  EXPOSE 443

  FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
  WORKDIR /src
  COPY ["GiacenzaSorterRm.csproj", "./"]
  RUN dotnet restore "GiacenzaSorterRm.csproj"
  COPY . .
  RUN dotnet build "GiacenzaSorterRm.csproj" -c Release -o /app/build

  FROM build AS publish
  RUN dotnet publish "GiacenzaSorterRm.csproj" -c Release -o /app/publish

  FROM base AS final
  WORKDIR /app
  COPY --from=publish /app/publish .
  ENTRYPOINT ["dotnet", "GiacenzaSorterRm.dll"]
  ```

- [ ] ?? **Creare Docker Compose per Dev Environment**
  
  **File:** `docker-compose.yml` (DA CREARE)
  ```yaml
  version: '3.8'
  services:
    web:
      build: .
      ports:
        - "5000:80"
      environment:
        - ASPNETCORE_ENVIRONMENT=Development
        - ConnectionStrings__DefaultConnection=Server=db;Database=GIACENZA_SORTER_RM;User Id=sa;Password=YourStrong@Passw0rd
      depends_on:
        - db
    
    db:
      image: mcr.microsoft.com/mssql/server:2022-latest
      environment:
        - ACCEPT_EULA=Y
        - SA_PASSWORD=YourStrong@Passw0rd
      ports:
        - "1433:1433"
      volumes:
        - sqldata:/var/opt/mssql
  
  volumes:
    sqldata:
  ```

#### 6.3 Environment Configuration
- [x] ? **Multi-Environment Support** - COMPLETATO
  - LocalDev
  - TestDev
  - Production

- [ ] ?? **Creare appsettings per ogni ambiente**
  - `appsettings.LocalDev.json`
  - `appsettings.TestDev.json`
  - `appsettings.Production.json`

#### 6.4 Health Checks
- [ ] ?? **Implementare Health Checks**
  ```csharp
  // In Startup.cs
  services.AddHealthChecks()
      .AddDbContextCheck<GiacenzaSorterRmTestContext>("database")
      .AddCheck("ActiveDirectory", () =>
      {
          // Verifica connettività AD
          return HealthCheckResult.Healthy();
      });
  
  // In Configure
  app.UseHealthChecks("/health", new HealthCheckOptions
  {
      ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
  });
  ```
  - Package: `AspNetCore.HealthChecks.SqlServer`
  - Package: `AspNetCore.HealthChecks.UI`

#### 6.5 Monitoring e APM
- [ ] ?? **Integrazione Application Insights**
  ```bash
  dotnet add package Microsoft.ApplicationInsights.AspNetCore
  ```
  ```csharp
  // In Program.cs
  .ConfigureServices((context, services) =>
  {
      services.AddApplicationInsightsTelemetry(
          context.Configuration["ApplicationInsights:InstrumentationKey"]);
  })
  ```
  - Telemetria automatica
  - Performance monitoring
  - Exception tracking

- [ ] ?? **Implementare Serilog per Structured Logging**
  ```bash
  dotnet add package Serilog.AspNetCore
  dotnet add package Serilog.Sinks.File
  dotnet add package Serilog.Sinks.Seq
  ```
  ```csharp
  Log.Logger = new LoggerConfiguration()
      .ReadFrom.Configuration(Configuration)
      .Enrich.FromLogContext()
      .WriteTo.Console()
      .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
      .WriteTo.Seq("http://localhost:5341")
      .CreateLogger();
  ```

#### 6.6 Database Migrations
- [ ] ?? **Implementare EF Core Migrations**
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
  - Versionare schema database
  - Rollback capabilities
  - Script SQL generabili

---

## 7. Miglioramenti Testing

### ?? PRIORITÀ MEDIA

#### 7.1 Unit Testing
- [ ] ?? **Creare Progetto Unit Test**
  ```bash
  dotnet new xunit -n GiacenzaSorterRm.Tests
  dotnet add GiacenzaSorterRm.Tests package Moq
  dotnet add GiacenzaSorterRm.Tests package FluentAssertions
  ```

- [ ] ?? **Test per Servizi di Autenticazione**
  ```csharp
  public class AuthenticationServiceTests
  {
      [Fact]
      public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
      {
          // Arrange
          var mockLogger = new Mock<ILogger<AuthenticationService>>();
          var mockCache = new Mock<IMemoryCache>();
          var mockAdService = new Mock<IActiveDirectoryService>();
          var mockOptions = Options.Create(new AuthenticationSettings());
          
          var service = new AuthenticationService(
              mockLogger.Object,
              mockCache.Object,
              mockAdService.Object,
              mockOptions.Object);
          
          var user = new Operatori 
          { 
              Username = "test", 
              Password = BCrypt.Net.BCrypt.HashPassword("password123") 
          };
          
          // Act
          var result = await service.AuthenticateAsync(user, "password123");
          
          // Assert
          result.Should().BeTrue();
      }
  }
  ```

- [ ] ?? **Test Coverage > 80%**
  - Servizi business logic
  - Repositories
  - Validators

#### 7.2 Integration Testing
- [ ] ?? **Test Integration con Database In-Memory**
  ```csharp
  public class ScatoleRepositoryIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
  {
      private readonly WebApplicationFactory<Startup> _factory;
      
      public ScatoleRepositoryIntegrationTests(WebApplicationFactory<Startup> factory)
      {
          _factory = factory;
      }
      
      [Fact]
      public async Task GetScatolaByCodice_ExistingCodice_ReturnsScatola()
      {
          // Test con database in-memory
      }
  }
  ```

#### 7.3 End-to-End Testing
- [ ] ?? **Implementare Playwright per E2E Tests**
  ```bash
  dotnet add package Microsoft.Playwright
  ```
  ```csharp
  [Test]
  public async Task LoginFlow_ValidCredentials_RedirectsToHome()
  {
      using var playwright = await Playwright.CreateAsync();
      await using var browser = await playwright.Chromium.LaunchAsync();
      var page = await browser.NewPageAsync();
      
      await page.GotoAsync("http://localhost:5000");
      await page.FillAsync("#Username", "admin");
      await page.FillAsync("#Password", "password");
      await page.ClickAsync("button[type='submit']");
      
      await page.WaitForURLAsync("**/Home");
      // Assert...
  }
  ```

#### 7.4 Load Testing
- [ ] ?? **Implementare Load Tests con K6**
  ```javascript
  // load-test.js
  import http from 'k6/http';
  import { check } from 'k6';
  
  export let options = {
      stages: [
          { duration: '2m', target: 100 }, // Ramp up to 100 users
          { duration: '5m', target: 100 }, // Stay at 100 users
          { duration: '2m', target: 0 },   // Ramp down
      ],
  };
  
  export default function () {
      let res = http.get('http://localhost:5000/Home');
      check(res, {
          'status is 200': (r) => r.status === 200,
          'response time < 500ms': (r) => r.timings.duration < 500,
      });
  }
  ```

---

## 8. Miglioramenti Logging e Monitoring

### ?? PRIORITÀ MEDIA

#### 8.1 Structured Logging
- [x] ? **NLog Configurato** - GIÀ IMPLEMENTATO

- [ ] ?? **Migliorare NLog Configuration**
  ```xml
  <!-- nlog.config -->
  <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd">
    <targets>
      <target name="jsonfile" 
              xsi:type="File" 
              fileName="logs/giacenza-${shortdate}.json">
        <layout xsi:type="JsonLayout">
          <attribute name="time" layout="${longdate}" />
          <attribute name="level" layout="${level:upperCase=true}"/>
          <attribute name="message" layout="${message}" />
          <attribute name="exception" layout="${exception:format=toString}" />
          <attribute name="user" layout="${aspnet-user-identity}" />
          <attribute name="url" layout="${aspnet-request-url}" />
        </layout>
      </target>
    </targets>
    
    <rules>
      <logger name="*" minlevel="Info" writeTo="jsonfile" />
    </rules>
  </nlog>
  ```

#### 8.2 Correlation IDs
- [ ] ?? **Implementare Request Correlation**
  ```csharp
  public class CorrelationIdMiddleware
  {
      private const string CorrelationIdHeader = "X-Correlation-ID";
      
      public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
      {
          var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
              ?? Guid.NewGuid().ToString();
          
          context.Response.Headers.Add(CorrelationIdHeader, correlationId);
          
          using (logger.BeginScope(new Dictionary<string, object>
          {
              ["CorrelationId"] = correlationId
          }))
          {
              await _next(context);
          }
      }
  }
  ```

#### 8.3 Performance Metrics
- [ ] ?? **Tracciare Metriche Chiave**
  - Tempo medio risposta per endpoint
  - Numero richieste al secondo
  - Tasso errori
  - Database query performance

#### 8.4 Alerting
- [ ] ?? **Configurare Alerting**
  - Email alert per errori critici
  - Slack/Teams notification per deploy
  - Threshold alerts (CPU > 80%, Memory > 90%)

---

## 9. Miglioramenti Database

### ?? PRIORITÀ BASSA

#### 9.1 Schema Optimization
- [ ] ?? **Code Review Database Schema**
  - Normalizzazione corretta (evitare ridondanza)
  - Foreign Keys correttamente definite
  - Check constraints per validazione dati

#### 9.2 Indexing Strategy
- [ ] ?? **Analizzare Query Plan e Creare Indici**
  ```sql
  -- Esempio: Indice per query frequenti su Scatole
  CREATE NONCLUSTERED INDEX IX_Scatole_CommessaData
  ON Scatole(IdCommessa, DataInserimento)
  INCLUDE (CodiceScatola, StatoScatola);
  ```

- [ ] ?? **Monitorare Missing Indexes**
  ```sql
  -- Query per identificare indici mancanti
  SELECT 
      d.statement AS TableName,
      d.equality_columns,
      d.inequality_columns,
      d.included_columns,
      s.avg_total_user_cost,
      s.avg_user_impact
  FROM sys.dm_db_missing_index_details d
  JOIN sys.dm_db_missing_index_groups g ON d.index_handle = g.index_handle
  JOIN sys.dm_db_missing_index_group_stats s ON g.index_group_handle = s.group_handle
  ORDER BY s.avg_user_impact DESC;
  ```

#### 9.3 Partitioning
- [ ] ?? **Valutare Partitioning per Tabelle Grandi**
  - Partitioning per data (Scatole, Bancali)
  - Migliorare performance query su range temporali

#### 9.4 Archiving Strategy
- [ ] ?? **Implementare Data Archiving**
  - Spostare dati vecchi (> 2 anni) su tabella archivio
  - Job SQL Agent automatico
  - Mantenere performance su tabelle operative

#### 9.5 Backup and Recovery
- [ ] ?? **Documentare Backup Strategy**
  - Full backup giornaliero
  - Differential backup ogni 6 ore
  - Transaction log backup ogni ora
  - Retention policy: 30 giorni

---

## 10. Documentazione e Manutenibilità

### ?? PRIORITÀ BASSA

#### 10.1 Code Documentation
- [ ] ?? **Aggiungere XML Comments**
  ```csharp
  /// <summary>
  /// Autentica un operatore verificando le credenziali fornite.
  /// </summary>
  /// <param name="user">L'operatore da autenticare</param>
  /// <param name="password">La password in chiaro da verificare</param>
  /// <returns>True se autenticazione riuscita, false altrimenti</returns>
  public async Task<bool> AuthenticateAsync(Operatori user, string password)
  {
      // Implementation...
  }
  ```

- [ ] ?? **Generare Documentazione API con Swagger**
  ```bash
  dotnet add package Swashbuckle.AspNetCore
  ```
  ```csharp
  services.AddSwaggerGen(c =>
  {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
          Title = "GiacenzaSorter API",
          Version = "v1",
          Description = "API per gestione giacenza"
      });
      
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      c.IncludeXmlComments(xmlPath);
  });
  ```

#### 10.2 README.md
- [ ] ?? **Migliorare README.md**
  - Descrizione progetto
  - Prerequisiti
  - Setup development environment
  - Guida contribuzione
  - FAQ

#### 10.3 Architecture Decision Records (ADR)
- [ ] ?? **Creare ADR per Decisioni Architetturali**
  ```markdown
  # ADR-001: Uso di Repository Pattern
  
  ## Status
  Accettato
  
  ## Context
  Codice attualmente accoppiato a Entity Framework direttamente nei PageModel.
  
  ## Decision
  Implementare Repository Pattern per:
  - Separare business logic da data access
  - Migliorare testabilità
  - Permettere cambio ORM futuro
  
  ## Consequences
  - Più codice boilerplate
  - Migliore manutenibilità
  ```

#### 10.4 User Manual
- [ ] ?? **Creare Manuale Utente**
  - Guida operazioni comuni
  - Screenshots
  - Troubleshooting

#### 10.5 Developer Guide
- [ ] ?? **Creare Developer Guide**
  - Coding standards
  - Git workflow
  - Branching strategy
  - Code review checklist

---

## ?? Riepilogo Priorità

### ?? PRIORITÀ ALTA (Da fare SUBITO)

1. ? **Sicurezza Autenticazione** - COMPLETATO
2. **Migrazione jQuery Ajax a Fetch API** (2-3 giorni)
3. **Security Headers e HTTPS Enforcement** (1 giorno)
4. **Rate Limiting Avanzato** (1 giorno)
5. **Input Validation Robusta** (2 giorni)

**Tempo stimato: 1-2 settimane**

---

### ?? PRIORITÀ MEDIA (Prossimi 1-2 mesi)

6. **Repository Pattern** (1 settimana)
7. **Service Layer** (1 settimana)
8. **CI/CD Pipeline** (1 settimana)
9. **Unit Testing** (2 settimane)
10. **Performance Optimization** (1 settimana)

**Tempo stimato: 6-8 settimane**

---

### ?? PRIORITÀ BASSA (Backlog)

11. UI/UX Improvements
12. Advanced Monitoring
13. Database Optimization
14. Documentation

**Tempo stimato: Ongoing**

---

## ?? Metriche di Successo

### Sicurezza
- ? Zero vulnerabilità critiche
- ? Audit logging completo
- ? Secrets management sicuro
- ?? Penetration test superato

### Performance
- ?? Tempo risposta < 200ms (p95)
- ?? Database query < 50ms (p95)
- ?? Page load < 2 secondi

### Qualità Codice
- ?? Test coverage > 80%
- ?? Code smell = 0 (SonarQube)
- ?? Technical debt < 5%

### Manutenibilità
- ?? Documentazione API completa
- ?? Onboarding nuovo dev < 1 giorno
- ?? Time to fix bug < 4 ore

---

## ?? Strumenti Consigliati

### Development
- Visual Studio 2022
- Visual Studio Code
- Postman / Insomnia
- SQL Server Management Studio

### Testing
- xUnit
- Moq
- FluentAssertions
- Playwright
- K6

### DevOps
- GitHub Actions
- Docker Desktop
- Azure DevOps
- Seq (log analysis)

### Monitoring
- Application Insights
- Grafana
- Prometheus
- Seq

---

## ?? Supporto

Per domande o chiarimenti su questo documento:

1. Consultare documentazione esistente in `/DOCS`
2. Aprire issue su GitHub
3. Contattare team di sviluppo

---

**Ultima modifica**: 2025-01-XX  
**Versione documento**: 2.0  
**Autore**: GitHub Copilot  
**Stato**: ? Completo

---

## ?? Note Finali

Questo documento rappresenta una roadmap completa per portare GiacenzaSorterRm a standard enterprise.
Le priorità possono essere adattate in base a:
- Budget disponibile
- Risorse team
- Urgenza business
- Vincoli tecnici

**Raccomandazione**: Procedere con approccio iterativo, completando una priorità alla volta e validando i risultati prima di procedere.

Buon lavoro! ??
