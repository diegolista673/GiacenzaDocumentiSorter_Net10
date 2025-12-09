using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipoPiattaforme
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
        public Piattaforme Piattaforme { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Piattaforme = await _context.Piattaformes.FirstOrDefaultAsync(m => m.IdPiattaforma == id);

            if (Piattaforme == null)
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

            Piattaforme = await _context.Piattaformes.FindAsync(id);

            if (Piattaforme != null)
            {
                _context.Piattaformes.Remove(Piattaforme);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Piattaforma Eliminata: {@Piattaforme} by Utente: {Utente}", Piattaforme, User.Identity.Name);
            }

            return RedirectToPage("./Index");
        }
    }
}
