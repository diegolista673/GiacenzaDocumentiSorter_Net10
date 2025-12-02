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

namespace GiacenzaSorterRm.Pages.TipiContenitori
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class DeleteModel : PageModel    
    {
        private readonly GiacenzaSorterContext _context;

        public DeleteModel(GiacenzaSorterContext context)
        {
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

            Contenitori = await _context.Contenitoris.FirstOrDefaultAsync(m => m.IdContenitore == id);

            if (Contenitori == null)
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

            Contenitori = await _context.Contenitoris.FindAsync(id);

            if (Contenitori != null)
            {
                _context.Contenitoris.Remove(Contenitori);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
