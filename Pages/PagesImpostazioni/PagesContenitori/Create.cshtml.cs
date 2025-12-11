using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesImpostazioni.PagesContenitori
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
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
            ViewData["Title"] = "Create Contenitore";
            return Page();
        }

        [BindProperty]
        public Contenitori Contenitori { get; set; } = new Contenitori();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Contenitori.DataCreazione = DateTime.Now.Date;
                
                string? idOperatoreValue = User.FindFirst("IdOperatore")?.Value;
                if (int.TryParse(idOperatoreValue, out int idOperatore))
                {
                    Contenitori.IdOperatoreCreazione = idOperatore;
                }

                _context.Contenitoris.Add(Contenitori);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Contenitore creato: {Contenitore}", Contenitori.Contenitore);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. The name is already in use.");
                return Page();
            }
        }
    }
}
