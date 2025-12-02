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
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Pages.PagesRiepilogo
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


        public string Ruolo { get; set; }
        public string Utente { get; set; }
        public string Message { get; set; }
        public string Fase { get; set; }
        public List<SelectListItem> LstCentri { get; set; }
        public SelectList CommesseSL { get; set; }




        [BindProperty]
        [Required(ErrorMessage = "Centro is required")]
        [Range(1, int.MaxValue,ErrorMessage = "Centro is required")]
        public int SelectedCentro { get; set; }


        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;


        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;


        [BindProperty]
        public List<ScatolaView> LstScatoleView { get; set; }
  

        [Required(ErrorMessage = "Fase is required")]
        [BindProperty]
        public string? SelectedFase { get; set; }


        
        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Commessa is required")]
        [Required]
        public int? IdCommessa { get; set; }






        public async Task<IActionResult> OnGet()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;


            if (User.FindFirst("Azienda").Value == "POSTEL")
            {
                LstCentri = await _context.CentriLavs.Select(a => new SelectListItem
                {
                    Value = a.IdCentroLavorazione.ToString(),
                    Text = a.CentroLavDesc
                }).OrderBy(x => x.Value).ToListAsync();
            }
            else
            {
                int CentroID = CentroAppartenenza.SetCentroByUser(User);
                LstCentri = await _context.CentriLavs.Where(x => x.IdCentroLavorazione == CentroID).Select(a => new SelectListItem
                {
                    Value = a.IdCentroLavorazione.ToString(),
                    Text = a.CentroLavDesc
                }).OrderBy(x => x.Value).ToListAsync();
            }



            var sel = await _context.Commesses.OrderBy(x => x.Commessa).ToListAsync();

            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");

            return Page();
        }



        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;

            try
            {
                int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);
                                                                         

                if (ModelState.IsValid)
                {
                    if (SelectedFase == "NORMALIZZAZIONE")
                    {
                        await SetSearch(StartDate, EndDate, "normalizzazione", CentroID);
                        Fase = "Normalizzate";
                    }
                    else
                    {
                        await SetSearch(StartDate, EndDate, "sorter", CentroID);
                        Fase = "Sorterizzate";
                    }


                    return Partial("_RiepilogoScatole", this);
                }
                else
                {
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Page();
            }
        }




        public async Task<IActionResult> OnPostElimina()
        {
            var lst = LstScatoleView.Where(x => x.Elimina == true).ToList();
            if(lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    Scatole scatola = await _context.Scatoles.FindAsync(item.IdScatola);
                    if (scatola != null)
                    {
                        _context.Scatoles.Remove(scatola);

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Delete scatola normalizzata : " + scatola.Scatola + " - operatore : " + User.Identity.Name);

                        await SetSearch(StartDate, EndDate, "sorter", (int)scatola.IdCentroNormalizzazione);
                        return Partial("_RiepilogoScatole", this);
                    }
                }

            }

            return new EmptyResult();


        }



        public async Task SetSearch( DateTime startDate, DateTime endDate, string flag, int idCentro)
        {
   
            var query = new List<Scatole>();
            IQueryable<Scatole> lstScatole = query.AsQueryable();


            //tutti i centri and tutte le commesse and flag = "normalizzazione"
            if (idCentro == 5 & IdCommessa == 999 & flag == "normalizzazione")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate).AsQueryable();
            }
            
            
            //tutti i centri and tutte le commesse and flag = "sorter"
            if (idCentro == 5 & IdCommessa == 999 & flag == "sorter")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate).AsQueryable();
            }


            //Per tutti i centri di lavorazione and 1 commessa e flag = "normalizzazione"
            if (idCentro == 5 & IdCommessa > 0 & IdCommessa < 999 & flag == "normalizzazione")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate && x.IdCommessa == IdCommessa).AsQueryable();
            }



            //Per tutti i centri di lavorazione and 1 commessa  e flag = "sorter"
            if (idCentro == 5 & IdCommessa > 0 & IdCommessa < 999 & flag == "sorter")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate && x.IdCommessa == IdCommessa).AsQueryable();
            }



            //Per 1 centro di lavorazione and tutte le commesse e flag = "normalizzazione"
            if (idCentro != 5 & IdCommessa == 999 & flag == "normalizzazione")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate && x.IdCentroNormalizzazione == idCentro).AsQueryable();
            }


            //Per 1 centro di lavorazione and tutte le commesse e flag = "sorter"
            if (idCentro != 5 & IdCommessa == 999 & flag == "sorter")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate && x.IdCentroSorterizzazione == idCentro).AsQueryable();
            }


            //Per centro di lavorazione e per 1 commessa e flag = "normalizzazione"
            if (idCentro != 5 & IdCommessa > 0 & IdCommessa < 999 & flag == "normalizzazione")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate && x.IdCentroNormalizzazione == idCentro && x.IdCommessa == IdCommessa)
                                              .AsQueryable();
            }
            
                 
            
            //Per centro di lavorazione e per 1 commessa e flag = "sorter"
            if (idCentro != 5 & IdCommessa > 0 & IdCommessa < 999 & flag == "sorter")
            {
                lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate && x.IdCentroSorterizzazione == idCentro && x.IdCommessa == IdCommessa)
                                    .AsQueryable();
            }


            lstScatole.Include(s => s.IdCommessaNavigation)
                      .Include(s => s.IdContenitoreNavigation)
                      .Include(s => s.IdStatoNavigation)
                      .Include(s => s.IdTipoNormalizzazioneNavigation)
                      .Include(s => s.IdCentroNormalizzazioneNavigation)
                      .Include(s => s.IdCentroSorterizzazioneNavigation)
                      .Include(s => s.IdTipologiaNavigation)
                      .AsQueryable();

            LstScatoleView = await lstScatole.Select(m => new ScatolaView
                                {
                                    IdScatola = m.IdScatola,
                                    Scatola = m.Scatola,
                                    DataNormalizzazione = m.DataNormalizzazione,
                                    OperatoreNormalizzazione = m.OperatoreNormalizzazione,
                                    DataSorter = m.DataSorter,
                                    OperatoreSorter = m.OperatoreSorter,
                                    Note = m.Note,
                                    Stato = m.IdStatoNavigation.Stato,
                                    Contenitore = m.IdContenitoreNavigation.Contenitore,
                                    Commessa = m.IdCommessaNavigation.Commessa,
                                    Tipologia = m.IdTipologiaNavigation.Tipologia,
                                    TipoProdotto= m.IdTipoNormalizzazioneNavigation.TipoNormalizzazione,
                                    TotaleScatola = (int)m.IdContenitoreNavigation.TotaleDocumenti,
                                    Elimina = false,
                                    IdCentroNormalizzazione = (int)m.IdCentroNormalizzazione,
                                    IdCentroSorterizzazione = m.IdCentroSorterizzazione,
                                    CentroNormalizzazione = m.IdCentroNormalizzazioneNavigation.CentroLavDesc,
                                    CentroSorterizzazione = m.IdCentroSorterizzazioneNavigation.CentroLavDesc
                                }).OrderBy(x => x.IdScatola).ToListAsync();


            if (LstScatoleView.Count == 0)
            {
                Message = "Not results found";
            }

        }

    }
}
