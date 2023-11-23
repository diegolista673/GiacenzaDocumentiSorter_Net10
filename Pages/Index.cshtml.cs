using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Shyjus.BrowserDetection;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;



namespace GiacenzaSorterRm.Pages
{
    public class IndexModel : PageModel
    {

        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IBrowserDetector browserDetector;

        [BindProperty]
        public string UserName { get; set; }
        
        [BindProperty, DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        public string Message { get; set; }
        
        [BindProperty]
        public string Centro { get; set; }

        [BindProperty]
        public CentriLav CentriLav { get; set; }

        public List<SelectListItem> SelectCentro { get; set; }


        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context, IBrowserDetector browserDetector)
        {
            
            _logger = logger;
            _logger.LogInformation("Pagina di Login");
            _context = context;
            this.browserDetector = browserDetector;

        }


        public IActionResult OnGet()
        {
            var browser = this.browserDetector.Browser;
            if (browser.Name== BrowserNames.InternetExplorer)
            {
                return RedirectToPage("/IndexBrowser");
            }

            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            if (String.IsNullOrEmpty(UserName))
            {
                Message = "Login failed";
                return Page();
            }

            if (String.IsNullOrEmpty(Password))
            {
                Message = "Login failed";
                return Page();
            }



            //var user = await _context.Operatoris.Include(s => s.IdCentroLavNavigation).Where(x => x.Operatore == UserName).AsNoTracking().FirstOrDefaultAsync();
            //if (user != null)
            //{

            //    var passwordHasher = new PasswordHasher<string>();
            //    var state = passwordHasher.VerifyHashedPassword(null, user.Password, Password);

            //    if (state == PasswordVerificationResult.Success || state == PasswordVerificationResult.SuccessRehashNeeded)
            //    {

            //        var claims = new List<Claim>
            //        {
            //            new Claim(ClaimTypes.Name, UserName)
            //        };

            //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //        var ruolo = new Claim(claimsIdentity.RoleClaimType, user.Ruolo);
            //        claimsIdentity.AddClaim(ruolo);


            //        var azienda = new Claim("Azienda", user.Azienda);
            //        claimsIdentity.AddClaim(azienda);

            //        var idOper = new Claim("IdOperatore", user.IdOperatore.ToString());
            //        claimsIdentity.AddClaim(idOper);

            //        var idCentro = new Claim("idCentro", user.IdCentroLav.ToString());
            //        claimsIdentity.AddClaim(idCentro);

            //        var Site = new Claim("Centro", user.IdCentroLavNavigation.CentroLavDesc.ToString());
            //        claimsIdentity.AddClaim(Site);

            //        var Ruolo = new Claim("Ruolo", user.Ruolo.ToString());
            //        claimsIdentity.AddClaim(Ruolo);

            //        var authProperties = new AuthenticationProperties
            //        {
            //            //AllowRefresh = true,
            //            // Refreshing the authentication session should be allowed.

            //            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4),
            //            // The time at which the authentication ticket expires. A 
            //            // value set here overrides the ExpireTimeSpan option of 
            //            // CookieAuthenticationOptions set with AddCookie.

            //            //IsPersistent = true,
            //            // Whether the authentication session is persisted across 
            //            // multiple requests. When used with cookies, controls
            //            // whether the cookie's lifetime is absolute (matching the
            //            // lifetime of the authentication ticket) or session-based.

            //            //IssuedUtc = <DateTimeOffset>,
            //            // The time at which the authentication ticket was issued.

            //            //RedirectUri = "./Index"
            //            // The full path or absolute URI to be used as an http 
            //            // redirect response value.
            //        };

            //        //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            //        await HttpContext.SignInAsync(
            //            CookieAuthenticationDefaults.AuthenticationScheme,
            //            new ClaimsPrincipal(claimsIdentity),
            //            authProperties);



            //        return RedirectToPage("/Home");
            //    }
            //}





            //set Development ( per fase di debug ) or set Production in launchSettings.json
            //set property in project for production
            //<PropertyGroup>
            //< EnvironmentName > Production </ EnvironmentName >
            //</ PropertyGroup >

            var user = await _context.Operatoris.Include(s => s.IdCentroLavNavigation).Where(x => x.Operatore == UserName).AsNoTracking().FirstOrDefaultAsync();

            if (user != null)
            {
                var claims = new List<Claim>
                                        {
                                            new Claim(ClaimTypes.Name, UserName)
                                        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var ruolo = new Claim(claimsIdentity.RoleClaimType, user.Ruolo);
                claimsIdentity.AddClaim(ruolo);


                var azienda = new Claim("Azienda", user.Azienda);
                claimsIdentity.AddClaim(azienda);

                var idOper = new Claim("IdOperatore", user.IdOperatore.ToString());
                claimsIdentity.AddClaim(idOper);

                var idCentro = new Claim("idCentro", user.IdCentroLav.ToString());
                claimsIdentity.AddClaim(idCentro);

                var Site = new Claim("Centro", user.IdCentroLavNavigation.CentroLavDesc.ToString());
                claimsIdentity.AddClaim(Site);

                var Ruolo = new Claim("Ruolo", user.Ruolo.ToString());
                claimsIdentity.AddClaim(Ruolo);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = "./Index"
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


            if (env == "Development")
            {
                if (user != null)
                {
                    return RedirectToPage("/Home");
                }
            }


            if (env == "Production")
            {
                if (user.Azienda == "ESTERNO")
                {
                    var passwordHasher = new PasswordHasher<string>();
                    var state = passwordHasher.VerifyHashedPassword(null, user.Password, Password);

                    if (state == PasswordVerificationResult.Success || state == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        return RedirectToPage("/Home");
                    }
                }
                else
                {
                    bool res = ActiveDirectoryAuthenticate(UserName, Password);
                    if (res)
                    {
                        if (user != null)
                        {
                            return RedirectToPage("/Home");
                        }
                    }
                }

            }
        

            Message = "Login failed";
            return Page();
        }


        public bool ActiveDirectoryAuthenticate(string username, string password)
        {
            bool result = false;

            using (System.DirectoryServices.DirectoryEntry _entry = new System.DirectoryServices.DirectoryEntry())
            {
                _entry.Username = username;
                _entry.Password = password;


                DirectorySearcher _searcher = new DirectorySearcher(_entry);
                _searcher.Filter = "(objectclass=user)";
                try
                {
                    SearchResult _sr = _searcher.FindOne();

                    string _name = _sr.Properties["displayname"][0].ToString();

                    result = true;
                }
                catch
                { /* Error handling omitted to keep code short: remember to handle exceptions !*/ }
            }


            return result; //true = user authenticated!
        }

    }

}


