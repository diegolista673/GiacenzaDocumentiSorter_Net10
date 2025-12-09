using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using System.Linq;
using System.Threading.Tasks;


namespace GiacenzaSorterRm.Pages.TipiLavorazioni
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;

        public DeleteModel(ILogger<CreateModel> logger,GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Commesse Commesse { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Commesse = await _context.Commesses.Include(c => c.IdOperatoreNavigation).FirstOrDefaultAsync(m => m.IdCommessa == id);

            if (Commesse == null)
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

            Commesse = await _context.Commesses.FirstOrDefaultAsync(x=> x.IdCommessa == id);

            if (Commesse != null)
            {
                
                _context.Commesses.Remove(Commesse);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Commessa eliminata: {Commessa}", Commesse.Commessa);
            return RedirectToPage("./Index");
        }
    }
}
