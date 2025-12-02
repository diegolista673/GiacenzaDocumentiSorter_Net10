using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Pages.PagesOperatori
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;

        public DeleteModel(GiacenzaSorterContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Operatori Operatori { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Operatori = await _context.Operatoris.FirstOrDefaultAsync(m => m.IdOperatore == id);

            if (Operatori == null)
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

            Operatori = await _context.Operatoris.FindAsync(id);

            if (Operatori != null)
            {
                _context.Operatoris.Remove(Operatori);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
