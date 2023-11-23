using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using GiacenzaSorterRm.AppCode;

namespace GiacenzaSorterRm.Pages.TipiLavorazioni
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<IndexModel> _logger;

        public string Ruolo { get; set; }
        public string Utente { get; set; }

        public int CentroID { get; set; }

        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
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
