using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Models.Database;
using GiacenzaSorterRm.AppCode;

namespace GiacenzaSorterRm.Pages.TipiLavorazioni
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<IndexModel> _logger;

        public string Ruolo { get; set; }
        public string Utente { get; set; }

        public int CentroID { get; set; }

        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        
        public List<Commesse> Commesse { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CentroID = CentroAppartenenza.SetCentroByUser(User);
            Commesse = await _context.Commesses.Include(b => b.IdPiattaformaNavigation).ToListAsync();
            
            return Page();
        }
    }
}
