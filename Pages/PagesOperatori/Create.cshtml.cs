using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System;

namespace GiacenzaSorterRm.Pages.PagesOperatori
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<CreateModel> _logger;



        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }


        [BindProperty]
        public Operatori Operatori { get; set; }

        public SelectList CentriSL { get; set; }



        public IActionResult OnGet()
        {
            CentriSL = new SelectList(_context.CentriLavs.Where(x => x.IdCentroLavorazione != 5), "IdCentroLavorazione", "CentroLavDesc");
            ViewData["Title"] = "Create Operatore";
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

            try
            {

                var passwordHasher = new PasswordHasher<string>();
                var hash = passwordHasher.HashPassword(null, Operatori.Password);
                Operatori.Password = hash;

                _context.Operatoris.Add(Operatori);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The name is already in use.");
                return Page();
            }


            return RedirectToPage("./Index");
        }
    }
}
