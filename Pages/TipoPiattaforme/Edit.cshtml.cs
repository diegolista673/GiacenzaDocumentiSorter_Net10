using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace GiacenzaSorterRm.Pages.TipoPiattaforme
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class EditModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ILogger<EditModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Piattaforme Piattaforme { get; set; } = new Piattaforme();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Piattaforme? piattaforme = await _context.Piattaformes.FirstOrDefaultAsync(m => m.IdPiattaforma == id);

            if (piattaforme == null)
            {
                return NotFound();
            }

            Piattaforme = piattaforme;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Piattaforme.DataCreazione = DateTime.Now.Date;
            
            string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
            if (int.TryParse(idOperatoreValue, out int idOperatore))
            {
                Piattaforme.IdOperatoreCreazione = idOperatore;
            }

            _context.Attach(Piattaforme).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Piattaforma modificata: {Piattaforma} by Utente: {Utente}", 
                    Piattaforme.Piattaforma, User.Identity?.Name ?? "Unknown");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                if (!PiattaformeExists(Piattaforme.IdPiattaforma))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool PiattaformeExists(int id)
        {
            return _context.Piattaformes.Any(e => e.IdPiattaforma == id);
        }
    }
}
