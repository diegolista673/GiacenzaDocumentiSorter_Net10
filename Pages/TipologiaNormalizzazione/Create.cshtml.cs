using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
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
            return Page();
        }

        [BindProperty]
        public TipiNormalizzazione TipiNormalizzazione { get; set; } = new TipiNormalizzazione();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TipiNormalizzaziones.Add(TipiNormalizzazione);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tipologia Normalizzazione creata: {TipoNormalizzazione} by Utente: {Utente}", 
                TipiNormalizzazione.TipoNormalizzazione, User.Identity?.Name ?? "Unknown");
            
            return RedirectToPage("./Index");
        }
    }
}
