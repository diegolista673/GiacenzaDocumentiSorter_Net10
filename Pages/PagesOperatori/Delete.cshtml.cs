using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;


namespace GiacenzaSorterRm.Pages.PagesOperatori
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
        public Operatori Operatori { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Operatori = await _context.Operatoris.FirstOrDefaultAsync(m => m.IdOperatore == id);

            if (Operatori == null)
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

            Operatori = await _context.Operatoris.FindAsync(id);

            if (Operatori != null)
            {
                _context.Operatoris.Remove(Operatori);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Operatore Eliminato: {@Operatori} by Utente: {Utente}", Operatori, User.Identity.Name);
            }

            return RedirectToPage("./Index");
        }
    }
}
