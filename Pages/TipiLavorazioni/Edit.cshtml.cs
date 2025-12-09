using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.TipiLavorazioni
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
        public Commesse Commesse { get; set; } = new Commesse();

        public SelectList PiattaformeSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public int Number { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PiattaformeSL = new SelectList(_context.Piattaformes, "IdPiattaforma", "Piattaforma");

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Commesse.DataCreazione = DateTime.Now.Date;
            
            string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
            if (int.TryParse(idOperatoreValue, out int idOperatore))
            {
                Commesse.IdOperatore = idOperatore;
            }

            _context.Attach(Commesse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Commessa modificata: {Commessa}", Commesse.Commessa);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                if (!CommesseExists(Commesse.IdCommessa))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CommesseExists(int id)
        {
            return _context.Commesses.Any(e => e.IdCommessa == id);
        }
    }
}
