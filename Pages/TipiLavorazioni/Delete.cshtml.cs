using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Data;

namespace GiacenzaSorterRm.Pages.TipiLavorazioni
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly IAppDbContext _context;

        public DeleteModel(IAppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Commesse Commesse { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Commesse = await _context.Commesses.Include(c => c.IdOperatoreNavigation).FirstOrDefaultAsync(m => m.IdCommessa == id);

            if (Commesse == null)
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

            Commesse = await _context.Commesses.FirstOrDefaultAsync(x=> x.IdCommessa == id);

            if (Commesse != null)
            {
                
                _context.Commesses.Remove(Commesse);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
