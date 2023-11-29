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
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GiacenzaSorterRm.Pages.PagesOperatori
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class EditModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<EditModel> _logger;


        public EditModel(ILogger<EditModel> logger,  GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Operatori Operatori { get; set; }


        public SelectList CentriSL { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CentriSL = new SelectList(_context.CentriLavs.Where(x=> x.IdCentroLavorazione != 5), "IdCentroLavorazione", "CentroLavDesc");


            Operatori = await _context.Operatoris.Include(c => c.IdCentroLavNavigation).FirstOrDefaultAsync(m => m.IdOperatore == id);
              

            if (Operatori == null)
            {
                return NotFound();
            }

            ViewData["IdOperatoreCreazione"] = new SelectList(_context.Operatoris, "IdOperatore", "IdOperatore");
            return Page();
        }




        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var passwordHasher = new PasswordHasher<string>();
            var hash = passwordHasher.HashPassword(null, Operatori.Password);
            Operatori.Password = hash;

            _context.Attach(Operatori).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message.ToString());

                if (!OperatoriExists(Operatori.IdOperatore))
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

        private bool OperatoriExists(int id)
        {
            return _context.Operatoris.Any(e => e.IdOperatore == id);
        }
    }
}
