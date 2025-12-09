using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.PagesOperatori
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
        public Operatori Operatori { get; set; } = new Operatori();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Operatori? operatori = await _context.Operatoris.FirstOrDefaultAsync(m => m.IdOperatore == id);

            if (operatori == null)
            {
                return NotFound();
            }

            Operatori = operatori;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Operatori? operatori = await _context.Operatoris.FindAsync(id);

            if (operatori != null)
            {
                _context.Operatoris.Remove(operatori);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Operatore eliminato: {Operatore} by Utente: {Utente}", 
                    operatori.Operatore, User.Identity?.Name ?? "Unknown");
            }

            return RedirectToPage("./Index");
        }
    }
}
