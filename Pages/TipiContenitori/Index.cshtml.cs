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

namespace GiacenzaSorterRm.Pages.TipiContenitori
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;

        public IndexModel(GiacenzaSorterContext context)
        {
            _context = context;
        }

        public IList<Contenitori> Contenitori { get;set; }

        public async Task OnGetAsync()
        {

            Contenitori = await _context.Contenitoris.Include(d => d.IdOperatoreCreazioneNavigation ).ToListAsync();

        }
    }
}
