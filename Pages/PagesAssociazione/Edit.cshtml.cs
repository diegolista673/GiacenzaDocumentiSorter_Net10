using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesAssociazione
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class EditModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<EditModel> _logger;


        public EditModel(ILogger<EditModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
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



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }



            _context.Attach(Ctc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!CommessaTipologiaContenitoreExists(Ctc.IdRiepilogo))
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

        private bool CommessaTipologiaContenitoreExists(int id)
        {
            return _context.CommessaTipologiaContenitores.Any(e => e.IdTipologia == id);
        }
    }
}
