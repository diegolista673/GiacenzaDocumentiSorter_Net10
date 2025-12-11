using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesContenitori
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
        public Contenitori Contenitori { get; set; } = new Contenitori();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contenitori? contenitori = await _context.Contenitoris.FirstOrDefaultAsync(m => m.IdContenitore == id);

            if (contenitori == null)
            {
                return NotFound();
            }

            Contenitori = contenitori;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contenitori? contenitori = await _context.Contenitoris.FindAsync(id);

            if (contenitori != null)
            {
                _context.Contenitoris.Remove(contenitori);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contenitore eliminato: {Contenitore}", contenitori.Contenitore);
            }

            return RedirectToPage("./Index");
        }
    }
}
