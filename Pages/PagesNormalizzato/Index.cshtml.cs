using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using System.IO;
using ClosedXML.Excel;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.Models;
using GiacenzaSorterRm.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using GiacenzaSorterRm.AppCode;

namespace GiacenzaSorterRm.Pages.PagesNormalizzato
{
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
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
            

            LstCentri = await _context.CentriLavs.Where(x => x.IdCentroLavorazione != 5).Select(a => new SelectListItem
            {
                Value = a.IdCentroLavorazione.ToString(),
                Text = a.CentroLavDesc
            }).OrderBy(x => x.Text).ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo");

            int centroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);

            if (ModelState.IsValid)
            {

                var lstScatole = await _context.Scatoles
                                        .Include(s => s.IdCommessaNavigation)
                                        .Include(s => s.IdContenitoreNavigation)
                                        .Include(s => s.IdTipoNormalizzazioneNavigation)
                                        .Include(s => s.IdTipologiaNavigation).Where(x => x.DataNormalizzazione >= StartDate && x.DataNormalizzazione <= EndDate & x.IdCentroNormalizzazione == centroID)
                                        .AsNoTracking().ToListAsync();



                LstCommesseView = (from p in lstScatole
                                   group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia } into g
                                   select new CommesseView
                                   {
                                       Commessa = g.Key.Commessa,
                                       Tipologia = g.Key.Tipologia,
                                       TotaleDocumentiNormalizzati = g.Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),

                                   }).OrderBy(z => z.Commessa).ThenBy(z => z.Tipologia).ToList();


                if (LstCommesseView.Count == 0)
                {
                    Message = "Not results found";
                }

                return Partial("_RiepilogoNormalizzato", this);

            }
            else
            {
                return Page();
            }


        }


    }
}
