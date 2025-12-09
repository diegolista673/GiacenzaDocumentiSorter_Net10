using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipiDocumenti
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
        public Tipologie Tipologie { get; set; } = new Tipologie();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tipologie? tipologie = await _context.Tipologies.FirstOrDefaultAsync(m => m.IdTipologia == id);

            if (tipologie == null)
            {
                return NotFound();
            }

            Tipologie = tipologie;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tipologie? tipologie = await _context.Tipologies.FindAsync(id);

            if (tipologie != null)
            {
                _context.Tipologies.Remove(tipologie);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tipologia documento eliminata: {Tipologia}", tipologie.Tipologia);
            }

            return RedirectToPage("./Index");
        }
    }
}
