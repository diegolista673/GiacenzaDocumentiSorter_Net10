using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;

        public IndexModel(GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _context = context;
        }

        public IList<TipiNormalizzazione> TipiNormalizzazione { get;set; }

        public async Task OnGetAsync()
        {
            TipiNormalizzazione = await _context.TipiNormalizzaziones.ToListAsync();
        }
    }
}
