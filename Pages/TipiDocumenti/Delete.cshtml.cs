using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using System.Linq;
using System.Threading.Tasks;


namespace GiacenzaSorterRm.Pages.TipiDocumenti
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<EditModel> _logger;

        public DeleteModel(ILogger<EditModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Tipologie Tipologie { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tipologie = await _context.Tipologies.FirstOrDefaultAsync(m => m.IdTipologia == id);

            if (Tipologie == null)
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

            Tipologie = await _context.Tipologies.FindAsync(id);

            if (Tipologie != null)
            {
                _context.Tipologies.Remove(Tipologie);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Tipologia documento eliminata: {Tipologia}", Tipologie.Tipologia);
            return RedirectToPage("./Index");
        }
    }
}
