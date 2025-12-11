using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesTipiDocumenti
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
        public Tipologie Tipologie { get; set; } = new Tipologie();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tipologie? tipologie = await _context.Tipologies.FirstOrDefaultAsync(m => m.IdTipologia == id);

            if (tipologie == null)
            {
                return NotFound();
            }

            Tipologie = tipologie;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Tipologie.DataCreazione = DateTime.Now.Date;
            
            string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
            if (int.TryParse(idOperatoreValue, out int idOperatore))
            {
                Tipologie.IdOperatoreCreazione = idOperatore;
            }

            _context.Attach(Tipologie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tipologia modificata: {Tipologia}", Tipologie.Tipologia);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                if (!TipologieExists(Tipologie.IdTipologia))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool TipologieExists(int id)
        {
            return _context.Tipologies.Any(e => e.IdTipologia == id);
        }
    }
}