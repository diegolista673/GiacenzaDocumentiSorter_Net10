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
using GiacenzaSorterRm.Data;

namespace GiacenzaSorterRm.Pages.PagesSorterizzato
{
    [Authorize(Policy = "SorterRequirements")]
    public class IndexModel : PageModel
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IAppDbContext context)
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


        public string Message { get; set; }
        public List<CommesseView> LstCommesseView { get; set; }

        public string Ruolo { get; set; }
        public string Utente { get; set; }

        public List<SelectListItem> LstCentri { get; set; }

        [BindProperty]
        public int SelectedCentro { get; set; }



        public async Task<IActionResult> OnGet()
        {

            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;


            LstCentri = await _context.CentriLavs.Select(a => new SelectListItem
            {
                Value = a.IdCentroLavorazione.ToString(),
                Text = a.CentroLavDesc
            }).OrderBy(x => x.Value).ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;
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
                                        .AsNoTracking().Where(x => x.DataSorter >= StartDate && x.DataSorter <= EndDate && x.IdCentroSorterizzazione == CentroID).AsQueryable();
                }
                else
                {
                    lstScatole = _context.Scatoles
                                        .Include(s => s.IdTipologiaNavigation)
                                        .Include(s => s.IdCommessaNavigation)
                                        .Include(s => s.IdContenitoreNavigation)
                                        .AsNoTracking().Where(x => x.DataSorter >= StartDate && x.DataSorter <= EndDate ).AsQueryable();
                }




                LstCommesseView = await  (from p in lstScatole
                                           group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia } into g
                                           select new CommesseView
                                           {
                                               Commessa = g.Key.Commessa,
                                               Tipologia = g.Key.Tipologia,
                                               TotaleDocumentiSorterizzati = g.Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti)                             
                                           }).OrderBy(z => z.Commessa).ThenBy(z => z.Tipologia).ToListAsync();


                if (LstCommesseView.Count == 0)
                {
                    Message = "Not results found";
                }

                return Partial("_RiepilogoSorterizzato", this);

            }
            else
            {
                return Page();
            }


        }


    }
}
