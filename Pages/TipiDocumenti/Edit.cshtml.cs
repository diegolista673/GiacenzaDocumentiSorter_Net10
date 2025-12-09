using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


namespace GiacenzaSorterRm.Pages.TipiDocumenti
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
        public Tipologie Tipologie { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tipologie = await _context.Tipologies.FirstOrDefaultAsync(m => m.IdTipologia == id);

            if (Tipologie == null)
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

            Tipologie.DataCreazione = DateTime.Now.Date;
            Tipologie.IdOperatoreCreazione = Int32.Parse(User.FindFirst("IdOperatore").Value);
            _context.Attach(Tipologie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            _logger.LogInformation("Tipologia modificata: {Tipologia}", Tipologie.Tipologia);
            return RedirectToPage("./Index");
        }

        private bool TipologieExists(int id)
        {
            return _context.Tipologies.Any(e => e.IdTipologia == id);
        }
    }
}
