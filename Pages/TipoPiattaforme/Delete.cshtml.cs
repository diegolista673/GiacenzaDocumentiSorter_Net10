using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipoPiattaforme
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ILogger<DeleteModel> logger, GiacenzaSorterContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Piattaforme? piattaforme = await _context.Piattaformes.FindAsync(id);

            if (piattaforme != null)
            {
                _context.Piattaformes.Remove(piattaforme);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Piattaforma eliminata: {Piattaforma} by Utente: {Utente}", 
                    piattaforme.Piattaforma, User.Identity?.Name ?? "Unknown");
            }

            return RedirectToPage("./Index");
        }
    }
}
