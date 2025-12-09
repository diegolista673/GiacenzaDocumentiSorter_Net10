using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.TipiDocumenti
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;

        public IndexModel(GiacenzaSorterContext context)
        {
            _context = context;
        }

        public IList<Tipologie> Tipologie { get; set; } = new List<Tipologie>();

        public async Task OnGetAsync()
        {
            Tipologie = await _context.Tipologies.ToListAsync();
        }
    }
}
