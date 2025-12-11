using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesOperatori
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
        public Operatori Operatori { get; set; } = new Operatori();

        public SelectList CentriSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CentriSL = new SelectList(_context.CentriLavs.Where(x => x.IdCentroLavorazione != 5), 
                "IdCentroLavorazione", "CentroLavDesc");

            Operatori? operatori = await _context.Operatoris
                .Include(c => c.IdCentroLavNavigation)
                .FirstOrDefaultAsync(m => m.IdOperatore == id);

            if (operatori == null)
            {
                return NotFound();
            }

            Operatori = operatori;
            ViewData["IdOperatoreCreazione"] = new SelectList(_context.Operatoris, "IdOperatore", "IdOperatore");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Operatori.Azienda.ToLower() == "esterno")
            {
                var passwordHasher = new PasswordHasher<string>();
                var hash = passwordHasher.HashPassword(null, Operatori.Password);
                Operatori.Password = hash;
            }

            _context.Attach(Operatori).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error updating Operatore {IdOperatore}", Operatori.IdOperatore);

                if (!OperatoriExists(Operatori.IdOperatore))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool OperatoriExists(int id)
        {
            return _context.Operatoris.Any(e => e.IdOperatore == id);
        }
    }
}
