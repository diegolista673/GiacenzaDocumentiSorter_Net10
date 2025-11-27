using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Data;

namespace GiacenzaSorterRm.Pages.TipiContenitori
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<CreateModel> _logger;



        public CreateModel(ILogger<CreateModel> logger, IAppDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult OnGet()
        {
            ViewData["Title"] = "Create Contenitore";
            return Page();
        }

        [BindProperty]
        public Contenitori Contenitori { get; set; }

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
                Contenitori.DataCreazione = DateTime.Now.Date;
                Contenitori.IdOperatoreCreazione = Int32.Parse(User.FindFirst("IdOperatore").Value);
                _context.Contenitoris.Add(Contenitori);
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
