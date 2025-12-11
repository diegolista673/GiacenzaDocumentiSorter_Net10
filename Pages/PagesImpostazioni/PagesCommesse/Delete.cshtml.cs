using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesCommesse
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
        public Commesse Commesse { get; set; } = new Commesse();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Commesse? commesse = await _context.Commesses
                .Include(c => c.IdOperatoreNavigation)
                .FirstOrDefaultAsync(m => m.IdCommessa == id);

            if (commesse == null)
            {
                return NotFound();
            }

            Commesse = commesse;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Commesse? commesse = await _context.Commesses.FirstOrDefaultAsync(x => x.IdCommessa == id);

            if (commesse != null)
            {
                _context.Commesses.Remove(commesse);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Commessa eliminata: {Commessa}", commesse.Commessa);
            }

            return RedirectToPage("./Index");
        }
    }
}
