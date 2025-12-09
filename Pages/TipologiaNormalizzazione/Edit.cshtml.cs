using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TipiNormalizzazione).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tipologia Normalizzazione modificata: {TipoNormalizzazione} by Utente: {Utente}", 
                    TipiNormalizzazione.TipoNormalizzazione, User.Identity?.Name ?? "Unknown");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipiNormalizzazioneExists(TipiNormalizzazione.IdTipoNormalizzazione))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool TipiNormalizzazioneExists(int id)
        {
            return _context.TipiNormalizzaziones.Any(e => e.IdTipoNormalizzazione == id);
        }
    }
}
