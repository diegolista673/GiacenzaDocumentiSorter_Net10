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
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Pages.TipoPiattaforme
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;


        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }



        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Piattaforme Piattaforme { get; set; }

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
                Piattaforme.DataCreazione = DateTime.Now.Date;
                Piattaforme.IdOperatoreCreazione = Int32.Parse(User.FindFirst("IdOperatore").Value);
                _context.Piattaformes.Add(Piattaforme);
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
