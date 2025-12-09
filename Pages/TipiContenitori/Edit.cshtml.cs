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
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class EditModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<EditModel> _logger;


        public EditModel(ILogger<EditModel> logger,  GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Contenitori Contenitori { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contenitori = await _context.Contenitoris
                .Include(c => c.IdOperatoreCreazioneNavigation).FirstOrDefaultAsync(m => m.IdContenitore == id);

            if (Contenitori == null)
            {
                return NotFound();
            }
           ViewData["IdOperatoreCreazione"] = new SelectList(_context.Operatoris, "IdOperatore", "IdOperatore");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Contenitori.DataCreazione = DateTime.Now.Date;
            Contenitori.IdOperatoreCreazione = Int32.Parse(User.FindFirst("IdOperatore").Value);
            _context.Attach(Contenitori).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            _logger.LogInformation("Contenitore modificato: {Contenitore}", Contenitori.Contenitore);
            return RedirectToPage("./Index");
        }

        private bool ContenitoriExists(int id)
        {
            return _context.Contenitoris.Any(e => e.IdContenitore == id);
        }
    }
}
