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

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
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
        public TipiNormalizzazione TipiNormalizzazione { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TipiNormalizzazione = await _context.TipiNormalizzaziones.FirstOrDefaultAsync(m => m.IdTipoNormalizzazione == id);

            if (TipiNormalizzazione == null)
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

            TipiNormalizzazione = await _context.TipiNormalizzaziones.FindAsync(id);

            if (TipiNormalizzazione != null)
            {
                _context.TipiNormalizzaziones.Remove(TipiNormalizzazione);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
