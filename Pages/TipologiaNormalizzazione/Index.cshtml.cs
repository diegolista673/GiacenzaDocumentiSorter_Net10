using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Pages.TipologiaNormalizzazione
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;

        public IndexModel(GiacenzaSorterContext context)
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
