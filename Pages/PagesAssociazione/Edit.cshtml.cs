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
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
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

            _logger.LogInformation("Utente {User} ha modificato l'associazione ID {IdRiepilogo}", User.Identity.Name, Ctc.IdRiepilogo);

            return RedirectToPage("./Index");
        }

        private bool CommessaTipologiaContenitoreExists(int id)
        {
            return _context.CommessaTipologiaContenitores.Any(e => e.IdTipologia == id);
        }
    }
}
