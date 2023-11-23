using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;

namespace GiacenzaSorterRm.Pages.PagesAssociazione
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class DeleteModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;

        public DeleteModel(GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CommessaTipologiaContenitore Ctc { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ctc = await _context.CommessaTipologiaContenitores.FirstOrDefaultAsync(m => m.IdRiepilogo == id);

            if (Ctc == null)
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

                Ctc = await _context.CommessaTipologiaContenitores.FindAsync(id);

                if (Ctc != null)
                {
                    _context.CommessaTipologiaContenitores.Remove(Ctc);
                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("./Index");





        }
    }
}
