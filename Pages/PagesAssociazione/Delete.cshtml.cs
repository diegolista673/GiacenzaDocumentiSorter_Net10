using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;


namespace GiacenzaSorterRm.Pages.PagesAssociazione
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<IndexModel> _logger;

        public DeleteModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public CommessaTipologiaContenitore Ctc { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ctc = await _context.CommessaTipologiaContenitores.FirstOrDefaultAsync(m => m.IdRiepilogo == id);

            if (Ctc == null)
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

                Ctc = await _context.CommessaTipologiaContenitores.FindAsync(id);

                if (Ctc != null)
                {
                    _context.CommessaTipologiaContenitores.Remove(Ctc);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Associazione Eliminata: {@Ctc} by Utente: {Utente}", Ctc, User.Identity.Name);
                }

                return RedirectToPage("./Index");





        }
    }
}
