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
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Data;

namespace GiacenzaSorterRm.Pages.TipiLavorazioni
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class EditModel : PageModel
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<EditModel> _logger;


        public EditModel(ILogger<EditModel> logger, IAppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Commesse Commesse { get; set; }



        public SelectList PiattaformeSL { get; set; }

        public int Number { get; set; } = 1;



        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PiattaformeSL = new SelectList(_context.Piattaformes, "IdPiattaforma", "Piattaforma");

           
            Commesse = await _context.Commesses.Include(c => c.IdOperatoreNavigation).FirstOrDefaultAsync(m => m.IdCommessa == id);

            if (Commesse == null)
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


            Commesse.DataCreazione = DateTime.Now.Date;
            Commesse.IdOperatore  = Int32.Parse(User.FindFirst("IdOperatore").Value);
            _context.Attach(Commesse).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!CommesseExists(Commesse.IdCommessa))
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

        private bool CommesseExists(int id)
        {
            return _context.Commesses.Any(e => e.IdCommessa == id);
        }
    }
}
