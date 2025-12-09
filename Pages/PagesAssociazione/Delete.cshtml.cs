using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.PagesAssociazione
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CommessaTipologiaContenitore? ctc = await _context.CommessaTipologiaContenitores.FindAsync(id);

            if (ctc != null)
            {
                _context.CommessaTipologiaContenitores.Remove(ctc);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Associazione eliminata: ID {IdRiepilogo} by Utente: {Utente}", 
                    ctc.IdRiepilogo, User.Identity?.Name ?? "Unknown");
            }

            return RedirectToPage("./Index");
        }
    }
}
