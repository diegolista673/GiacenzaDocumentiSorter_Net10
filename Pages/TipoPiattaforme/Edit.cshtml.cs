using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;

namespace GiacenzaSorterRm.Pages.TipoPiattaforme
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Piattaforme.DataCreazione = DateTime.Now.Date;
            Piattaforme.IdOperatoreCreazione = Int32.Parse(User.FindFirst("IdOperatore").Value);
            _context.Attach(Piattaforme).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!TipologieExists(Piattaforme.IdPiattaforma))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TipologieExists(int id)
        {
            return _context.Piattaformes.Any(e => e.IdPiattaforma == id);
        }
    }
}
