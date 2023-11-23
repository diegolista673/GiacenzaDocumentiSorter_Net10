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

namespace GiacenzaSorterRm.Pages.PagesRiepilogoBancali
{
    [Authorize(Policy = "SorterRequirements")]
    public class EditModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<EditModel> _logger;


        public EditModel(ILogger<EditModel> logger,  GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        public SelectList CommesseSL { get; set; }


        [BindProperty]
        public Bancali Bancali { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Bancali = await _context.Bancalis.Where(m => m.IdBancale == id).FirstOrDefaultAsync(m => m.IdBancale == id);

            if (Bancali == null)
            {
                return NotFound();
            }

            CommesseSL = new SelectList(_context.Commesses, "IdCommessa", "Commessa");

            return Page();
        }



        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CommesseSL = new SelectList(_context.Commesses, "IdCommessa", "Commessa");
                return Page();
            }
           
           
            _context.Attach(Bancali).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Modified bancale : " + Bancali.Bancale + " - operatore : " + User.Identity.Name);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BancaleExists(Bancali.IdBancale))
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

        private bool BancaleExists(int id)
        {
            return _context.Bancalis.Any(e => e.IdBancale == id);
        }
    }
}
