using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesAssociazione
{
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;

        public IndexModel(GiacenzaSorterContext context)
        {
            _context = context;
        }

        public IList<CommessaTipologiaContenitore> CommessaTipologiaContenitore { get; set; } = new List<CommessaTipologiaContenitore>();

        public async Task OnGetAsync()
        {
            CommessaTipologiaContenitore = await _context.CommessaTipologiaContenitores.ToListAsync();
        }
    }
}
