using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using GiacenzaSorterRm.AppCode;

namespace GiacenzaSorterRm.Pages.PagesReports.PagesNormalizzato
{
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public string Message { get; set; } = string.Empty;
        
        public List<CommesseView> LstCommesseView { get; set; } = new List<CommesseView>();

        public string Ruolo { get; set; } = string.Empty;
        
        public string Utente { get; set; } = string.Empty;

        public List<SelectListItem> LstCentri { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public int SelectedCentro { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;

            LstCentri = await _context.CentriLavs.Select(a => new SelectListItem
            {
                Value = a.IdCentroLavorazione.ToString(),
                Text = a.CentroLavDesc
            }).OrderBy(x => x.Value).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;

            int centroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);

            if (ModelState.IsValid)
            {
                IQueryable<Scatole> lstScatole;

                if (centroID != 5)
                {
                    lstScatole = _context.Scatoles
                        .Include(s => s.IdCommessaNavigation)
                        .Include(s => s.IdContenitoreNavigation)
                        .Include(s => s.IdTipoNormalizzazioneNavigation)
                        .Include(s => s.IdTipologiaNavigation)
                        .Where(x => x.DataNormalizzazione >= StartDate && x.DataNormalizzazione <= EndDate && x.IdCentroNormalizzazione == centroID)
                        .AsNoTracking();
                }
                else
                {
                    lstScatole = _context.Scatoles
                        .Include(s => s.IdCommessaNavigation)
                        .Include(s => s.IdContenitoreNavigation)
                        .Include(s => s.IdTipoNormalizzazioneNavigation)
                        .Include(s => s.IdTipologiaNavigation)
                        .Where(x => x.DataNormalizzazione >= StartDate && x.DataNormalizzazione <= EndDate)
                        .AsNoTracking();
                }

                LstCommesseView = await (from p in lstScatole
                                         group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia } into g
                                         select new CommesseView
                                         {
                                             Commessa = g.Key.Commessa,
                                             Tipologia = g.Key.Tipologia,
                                             TotaleDocumentiNormalizzati = g.Sum(x => x.IdContenitoreNavigation.TotaleDocumenti ?? 0),
                                         })
                                         .OrderBy(z => z.Commessa)
                                         .ThenBy(z => z.Tipologia)
                                         .ToListAsync();

                if (LstCommesseView.Count == 0)
                {
                    Message = "Not results found";
                }

                _logger.LogInformation("User {User} generated Normalized Summary Report for IDCentro {Centro} from {StartDate} to {EndDate}", 
                    Utente, centroID, StartDate.ToShortDateString(), EndDate.ToShortDateString());
                
                return Partial("_RiepilogoNormalizzato", this);
            }
            else
            {
                return Page();
            }
        }
    }
}
