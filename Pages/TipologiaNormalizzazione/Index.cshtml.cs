using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GiacenzaSorterContext _context;

        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<TipiNormalizzazione> TipiNormalizzazione { get; set; } = new List<TipiNormalizzazione>();

        public async Task OnGetAsync()
        {
            TipiNormalizzazione = await _context.TipiNormalizzaziones.ToListAsync();
            _logger.LogInformation("Visualizzate Tipologie Normalizzazione da Utente: {Utente}", User.Identity?.Name ?? "Unknown");
        }
    }
}
