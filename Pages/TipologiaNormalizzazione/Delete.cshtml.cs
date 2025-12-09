using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
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
        public TipiNormalizzazione TipiNormalizzazione { get; set; } = new TipiNormalizzazione();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TipiNormalizzazione? tipiNormalizzazione = await _context.TipiNormalizzaziones
                .FirstOrDefaultAsync(m => m.IdTipoNormalizzazione == id);

            if (tipiNormalizzazione == null)
            {
                return NotFound();
            }

            TipiNormalizzazione = tipiNormalizzazione;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TipiNormalizzazione? tipiNormalizzazione = await _context.TipiNormalizzaziones.FindAsync(id);

            if (tipiNormalizzazione != null)
            {
                _context.TipiNormalizzaziones.Remove(tipiNormalizzazione);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tipologia Normalizzazione eliminata: {TipoNormalizzazione} by Utente: {Utente}", 
                    tipiNormalizzazione.TipoNormalizzazione, User.Identity?.Name ?? "Unknown");
            }

            return RedirectToPage("./Index");
        }
    }
}
