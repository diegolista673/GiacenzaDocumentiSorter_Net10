using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipoPiattaforme
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Piattaforme> Piattaforme { get; set; } = new List<Piattaforme>();

        public async Task OnGetAsync()
        {
            Piattaforme = await _context.Piattaformes.ToListAsync();
            _logger.LogInformation("Visualizzate Piattaforme da Utente: {Utente}", User.Identity?.Name ?? "Unknown");
        }
    }
}
