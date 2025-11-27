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
using GiacenzaSorterRm.Data;
using Shyjus.BrowserDetection;

namespace GiacenzaSorterRm.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IBrowserDetector browserDetector;
        private readonly GiacenzaSorterRm.Services.IAuthenticationService _authService;

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
            IAppDbContext context, 
            IBrowserDetector browserDetector,
            GiacenzaSorterRm.Services.IAuthenticationService authService)
        {
            _logger = logger;
            _context = context;
            this.browserDetector = browserDetector;
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
            var browser = this.browserDetector.Browser;
            if (browser.Name == BrowserNames.InternetExplorer)
            {
                return RedirectToPage("/IndexBrowser");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // 1. Recupera utente dal database
                var user = await _context.Operatoris
                    .Include(s => s.IdCentroLavNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Operatore == UserName);

                // 2. VERIFICA PASSWORD PRIMA DI CREARE SESSIONE
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

                // 3. CREA SESSIONE SOLO SE AUTENTICATO
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
                new Claim("Centro", user.IdCentroLavNavigation.CentroLavDesc),
                new Claim("Ruolo", user.Ruolo)
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


