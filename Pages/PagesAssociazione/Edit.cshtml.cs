using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesAssociazione
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
        public CommessaTipologiaContenitore Ctc { get; set; } = new CommessaTipologiaContenitore();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CommessaTipologiaContenitore? ctc = await _context.CommessaTipologiaContenitores
                .FirstOrDefaultAsync(m => m.IdRiepilogo == id);

            if (ctc == null)
            {
                return NotFound();
            }

            Ctc = ctc;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Ctc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Utente {User} ha modificato l'associazione ID {IdRiepilogo}", 
                    User.Identity?.Name ?? "Unknown", Ctc.IdRiepilogo);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                if (!CommessaTipologiaContenitoreExists(Ctc.IdRiepilogo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CommessaTipologiaContenitoreExists(int id)
        {
            return _context.CommessaTipologiaContenitores.Any(e => e.IdRiepilogo == id);
        }
    }
}
