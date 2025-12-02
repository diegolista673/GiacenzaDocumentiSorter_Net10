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

namespace GiacenzaSorterRm.Pages.PagesOperatori
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;

        public IndexModel(GiacenzaSorterContext context)
        {
            _context = context;
        }

        public IList<Operatori> Operatori { get;set; }

        public async Task OnGetAsync()
        {

            Operatori = await _context.Operatoris.Include(d => d.IdCentroLavNavigation).ToListAsync();

        }
    }
}
