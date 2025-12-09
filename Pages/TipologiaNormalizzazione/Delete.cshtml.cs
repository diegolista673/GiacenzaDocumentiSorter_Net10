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
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<EditModel> _logger;

        public DeleteModel(ILogger<EditModel> logger,GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public TipiNormalizzazione TipiNormalizzazione { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TipiNormalizzazione = await _context.TipiNormalizzaziones.FirstOrDefaultAsync(m => m.IdTipoNormalizzazione == id);

            if (TipiNormalizzazione == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TipiNormalizzazione = await _context.TipiNormalizzaziones.FindAsync(id);

            if (TipiNormalizzazione != null)
            {
                _context.TipiNormalizzaziones.Remove(TipiNormalizzazione);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Tipologia Normalizzazione Eliminata: {@TipiNormalizzazione} by Utente: {Utente}", TipiNormalizzazione, User.Identity.Name);

            return RedirectToPage("./Index");
        }
    }
}
