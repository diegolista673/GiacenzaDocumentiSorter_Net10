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

namespace GiacenzaSorterRm.Pages.TipiContenitori
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
        public Contenitori Contenitori { get; set; } = new Contenitori();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contenitori? contenitori = await _context.Contenitoris
                .Include(c => c.IdOperatoreCreazioneNavigation)
                .FirstOrDefaultAsync(m => m.IdContenitore == id);

            if (contenitori == null)
            {
                return NotFound();
            }

            Contenitori = contenitori;
            ViewData["IdOperatoreCreazione"] = new SelectList(_context.Operatoris, "IdOperatore", "IdOperatore");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Contenitori.DataCreazione = DateTime.Now.Date;
            
            string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
            if (int.TryParse(idOperatoreValue, out int idOperatore))
            {
                Contenitori.IdOperatoreCreazione = idOperatore;
            }

            _context.Attach(Contenitori).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contenitore modificato: {Contenitore}", Contenitori.Contenitore);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContenitoriExists(Contenitori.IdContenitore))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ContenitoriExists(int id)
        {
            return _context.Contenitoris.Any(e => e.IdContenitore == id);
        }
    }
}
