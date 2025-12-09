using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GiacenzaSorterRm.Pages.PagesOperatori
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

        [BindProperty]
        public Operatori Operatori { get; set; } = new Operatori();

        public SelectList CentriSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public IActionResult OnGet()
        {
            CentriSL = new SelectList(_context.CentriLavs.Where(x => x.IdCentroLavorazione != 5), 
                "IdCentroLavorazione", "CentroLavDesc");
            ViewData["Title"] = "Create Operatore";
            return Page();
        }

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

                _logger.LogInformation("Operatore creato: {Operatore}", Operatori.Operatore);
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
