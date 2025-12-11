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
using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GiacenzaSorterRm.Pages.PagesReports.PagesSorterizzato
{
    [Authorize(Policy = "SorterRequirements")]
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
            Utente = User.Identity?.Name ?? string.Empty;
            int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);

            if (ModelState.IsValid)
            {
                IQueryable<Scatole> lstScatole;

                if (CentroID != 5)
                {
                    lstScatole = _context.Scatoles
                        .Include(s => s.IdTipologiaNavigation)
                        .Include(s => s.IdCommessaNavigation)
                        .Include(s => s.IdContenitoreNavigation)
                        .AsNoTracking()
                        .Where(x => x.DataSorter >= StartDate && x.DataSorter <= EndDate && x.IdCentroSorterizzazione == CentroID);
                }
                else
                {
                    lstScatole = _context.Scatoles
                        .Include(s => s.IdTipologiaNavigation)
                        .Include(s => s.IdCommessaNavigation)
                        .Include(s => s.IdContenitoreNavigation)
                        .AsNoTracking()
                        .Where(x => x.DataSorter >= StartDate && x.DataSorter <= EndDate);
                }

                LstCommesseView = await (from p in lstScatole
                                         group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia } into g
                                         select new CommesseView
                                         {
                                             Commessa = g.Key.Commessa,
                                             Tipologia = g.Key.Tipologia,
                                             TotaleDocumentiSorterizzati = g.Sum(x => x.IdContenitoreNavigation.TotaleDocumenti ?? 0)
                                         })
                                         .OrderBy(z => z.Commessa)
                                         .ThenBy(z => z.Tipologia)
                                         .ToListAsync();

                if (LstCommesseView.Count == 0)
                {
                    Message = "Not results found";
                }

                _logger.LogInformation("Utente {Utente} ha generato il report sorterizzato dal {StartDate} al {EndDate} per il centro {CentroID}", 
                    Utente, StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd"), CentroID);
                
                return Partial("_RiepilogoSorterizzato", this);
            }
            else
            {
                return Page();
            }
        }
    }
}
