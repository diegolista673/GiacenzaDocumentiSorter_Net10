using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesTipoPiattaforme
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Piattaforme Piattaforme { get; set; } = new Piattaforme();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Piattaforme.DataCreazione = DateTime.Now.Date;
                
                string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
                if (int.TryParse(idOperatoreValue, out int idOperatore))
                {
                    Piattaforme.IdOperatoreCreazione = idOperatore;
                }

                _context.Piattaformes.Add(Piattaforme);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Piattaforma creata: {Piattaforma} by Utente: {Utente}", 
                    Piattaforme.Piattaforma, User.Identity?.Name ?? "Unknown");
                
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. The name is already in use.");
                return Page();
            }
        }
    }
}