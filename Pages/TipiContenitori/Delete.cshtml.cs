using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;


namespace GiacenzaSorterRm.Pages.TipiContenitori
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
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
        public Contenitori Contenitori { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contenitori = await _context.Contenitoris.FirstOrDefaultAsync(m => m.IdContenitore == id);

            if (Contenitori == null)
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

            Contenitori = await _context.Contenitoris.FindAsync(id);

            if (Contenitori != null)
            {
                _context.Contenitoris.Remove(Contenitori);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Contenitore eliminato: {Contenitore}", Contenitori.Contenitore);
            return RedirectToPage("./Index");
        }
    }
}
