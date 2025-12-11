using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesCommesse
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<SelectListItem> LstCentri { get; set; } = new List<SelectListItem>();
        
        public string Ruolo { get; set; } = string.Empty;
        
        public string Utente { get; set; } = string.Empty;

        [BindProperty]
        public int SelectedCentro { get; set; }

        [BindProperty]
        public Commesse Commesse { get; set; } = new Commesse();

        public SelectList PiattaformeSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public IActionResult OnGet()
        {
            PiattaformeSL = new SelectList(_context.Piattaformes, "IdPiattaforma", "Piattaforma");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Commesse.DataCreazione = DateTime.Now.Date;
                
                string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
                if (int.TryParse(idOperatoreValue, out int idOperatore))
                {
                    Commesse.IdOperatore = idOperatore;
                }

                Commesse.GiorniSla = 1;
                Commesse.Attiva = true;

                _context.Commesses.Add(Commesse);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Commessa creata: {Commessa}", Commesse.Commessa);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. The name is already in use.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Commessa");
                ModelState.AddModelError(string.Empty, "An error occurred while saving.");
                return Page();
            }
        }
    }
}
