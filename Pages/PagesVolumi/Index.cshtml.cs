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
using GiacenzaSorterRm.Models.Database;
using Microsoft.Data.SqlClient;

namespace GiacenzaSorterRm.Pages.PagesVolumi
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

        //[BindProperty]
        //[Required]
        //[DataType(DataType.Date)]
        //public DateTime EndDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; } = DateTime.Now.Date;

        public string Message { get; set; }
        public List<CommesseView> LstCommesseView { get; set; }
        public int TotaleGiacenza { get; set; }

        public List<SelectListItem> LstCentri { get; set; }

        public string Ruolo { get; set; }
        public string Utente { get; set; }

        public List<GiacenzaView> LstGiacenzaView { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Centro is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Centro is required")]
        public int SelectedCentro { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "IdTipo is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IdTipo is required")]
        public int IdTipo { get; set; }

        public string ErrorMessage { get; set; }


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



        
        public async Task<IActionResult> ReportDettaglioAsync()
        {
            

            try
            {
                Ruolo = User.FindFirstValue("Ruolo");
                Utente = User.Identity.Name;
                int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);
                IQueryable<Scatole> lstSC;

                _context.Database.SetCommandTimeout(120);

                if (ModelState.IsValid)
                {

                    //se tutti
                    if (CentroID == 5)
                    {
                        //fatta cosi contiene tutte le commesse anche se a 0 in giacenza
                        //lstSC = _context.Scatoles
                        //                .Include(s => s.IdCommessaNavigation)
                        //                .Include(s => s.IdContenitoreNavigation)
                        //                .Include(s => s.IdStatoNavigation)
                        //                .Include(s => s.IdCentroGiacenzaNavigation)
                        //                .Include(s => s.IdTipologiaNavigation)
                        //                .Where(x => (x.DataNormalizzazione <= EndDate || x.DataSorter == EndDate ) && x.IdCommessaNavigation.Attiva == true && x.IdStato < 3  )
                        //                .AsNoTracking().AsQueryable();

                        lstSC = _context.Scatoles
                                        .Include(s => s.IdCommessaNavigation)
                                        .Include(s => s.IdContenitoreNavigation)
                                        .Include(s => s.IdStatoNavigation)
                                        .Include(s => s.IdCentroGiacenzaNavigation)
                                        .Include(s => s.IdTipologiaNavigation)
                                        .Where(s => (s.IdStato == 1 || s.DataSorter == EndDate) && s.IdCommessaNavigation.Attiva == true && s.IdStato < 3)
                                        .AsNoTracking().AsQueryable();
                    }
                    else
                    {
                        lstSC = _context.Scatoles
                                        .Include(s => s.IdCommessaNavigation)
                                        .Include(s => s.IdContenitoreNavigation)
                                        .Include(s => s.IdStatoNavigation)
                                        .Include(s => s.IdCentroGiacenzaNavigation)
                                        .Include(s => s.IdTipologiaNavigation)
                                        .Where(s => (s.IdStato == 1 || s.DataSorter == EndDate) && s.IdCommessaNavigation.Attiva == true && s.IdStato < 3 && s.IdCentroGiacenza == CentroID)
                                        .AsNoTracking().AsQueryable();
                    }

                    //NB:
                    //documenti normalizzati IN data selezionata
                    //documenti sorterizzati IN data selezionata
                    //totale giacenza FINO ALLA data selezionata
                    LstCommesseView = await (from p in lstSC
                                             group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia, p.IdCentroGiacenzaNavigation.CentroLavDesc, p.IdCommessaNavigation.IdPiattaformaNavigation.Piattaforma } into g
                                             select new CommesseView
                                             {
                                                 Commessa = g.Key.Commessa,
                                                 Tipologia = g.Key.Tipologia,
                                                 TotaleDocumentiNormalizzati = g.Where(x => x.IdStato == 1 && x.DataNormalizzazione == EndDate).Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),
                                                 TotaleDocumentiSorterizzati = g.Where(z => z.IdStato == 2 && z.DataSorter == EndDate).Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),
                                                 TotaleDocumentiGiacenza = g.Where(z => z.IdStato == 1).Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),
                                                 ScatolaNormalizzataOld = g.Where(p => p.IdStato == 1 && p.IdCommessaNavigation.Commessa == g.Key.Commessa && p.IdTipologiaNavigation.Tipologia == g.Key.Tipologia).OrderBy(x => x.DataNormalizzazione).Select(x => x.Scatola).FirstOrDefault(),
                                                 DataScatolaOld = g.Where(p => p.IdStato == 1 && p.IdCommessaNavigation.Commessa == g.Key.Commessa && p.IdTipologiaNavigation.Tipologia == g.Key.Tipologia).Select(x => x.DataNormalizzazione).Min(),
                                                 CentroGiacenza = g.Key.CentroLavDesc,
                                                 Piattaforma = g.Key.Piattaforma
                                             }).OrderBy(z => z.Commessa).ThenBy(z => z.Tipologia).ToListAsync();


                    //LstCommesseView = (from p in lstSC
                    //                   group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia } into g
                    //                   select new CommesseView
                    //                   {
                    //                       Commessa = g.Key.Commessa,
                    //                       Tipologia = g.Key.Tipologia,
                    //                       TotaleDocumentiNormalizzati = g.Where(x => x.DataNormalizzazione == EndDate).Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),
                    //                       TotaleDocumentiSorterizzati = g.Where(z => z.DataSorter == EndDate).Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),
                    //                       TotaleDocumentiGiacenza = g.Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti) - g.Where(z => z.IdStatoNavigation.Stato == "SORTERIZZATO").Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti) - g.Where(z => z.IdStatoNavigation.Stato == "MACERATO").Sum(x => (int)x.IdContenitoreNavigation.TotaleDocumenti),
                    //                       ScatolaNormalizzataOld = g.Where(p => p.IdCommessaNavigation.Commessa == g.Key.Commessa && p.IdTipologiaNavigation.Tipologia == g.Key.Tipologia && p.DataSorter == null).OrderBy(x => x.DataNormalizzazione).Select(x => x.Scatola).FirstOrDefault(),
                    //                       DataScatolaOld = g.Where(p => p.IdCommessaNavigation.Commessa == g.Key.Commessa && p.IdTipologiaNavigation.Tipologia == g.Key.Tipologia && p.DataSorter == null).OrderBy(x => x.DataNormalizzazione).Select(x => x.DataNormalizzazione).FirstOrDefault()

                    //                   }).OrderBy(z => z.Commessa).ThenBy(z => z.Tipologia).ToList();


                    TotaleGiacenza = LstCommesseView.Sum(x => x.TotaleDocumentiGiacenza);

                    if (LstCommesseView.Count == 0)
                    {
                        Message = "Not results found";
                    }

                    return Partial("_RiepilogoVolumi", this);


                }
                else
                {
                    return Page();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == -2)
                {
                    _logger.LogError(ex.Message);
                    ErrorMessage = ex.Message;
                }
                return Partial("_RiepilogoVolumi", this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ErrorMessage = ex.Message;
                return Partial("_RiepilogoVolumi", this);
            }
        }


        public async Task<IActionResult> ReportPiattaformaAsync()
        {
            try
            {
                Ruolo = User.FindFirstValue("Ruolo");
                Utente = User.Identity.Name;
                int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);

                _context.Database.SetCommandTimeout(120);

                if (ModelState.IsValid)
                {

                    LstGiacenzaView = new List<GiacenzaView>();
                    string sql = "";
                    string constr = _context.Database.GetDbConnection().ConnectionString;

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string fromDate = EndDate.Date.ToString("yyyyMMdd");

                        //se tutti
                        if (CentroID == 5)
                        {
                            sql = @"SELECT c1.CentroLavDesc as Centro,p.Piattaforma,sum(c0.TotaleDocumenti) as Giacenza
                                    FROM Scatole AS s
                                    INNER JOIN Commesse AS c ON s.IdCommessa = c.IdCommessa
                                    INNER JOIN Piattaforme AS p ON c.IdPiattaforma = p.IdPiattaforma
                                    INNER JOIN Contenitori AS c0 ON s.IdContenitore = c0.IdContenitore
                                    INNER JOIN CentriLav AS c1 ON s.IdCentroGiacenza = c1.IdCentroLavorazione
                                    WHERE s.DataNormalizzazione <= {0} and c.Attiva = 1 and s.IdStato = 1
                                    group by p.Piattaforma,c1.CentroLavDesc
                                    order by c1.CentroLavDesc,p.Piattaforma";

                            using (SqlCommand cmd = new SqlCommand(sql))
                            {

                                cmd.Connection = con;
                                cmd.CommandTimeout = 120;
                                cmd.Parameters.Clear();

                                string[] myParams = { fromDate };

                                LstGiacenzaView = await _context.Set<GiacenzaView>().FromSqlRaw(sql, myParams).ToListAsync();
                            }
                        }
                        else
                        {
                            sql = @"SELECT c1.CentroLavDesc as Centro,p.Piattaforma,sum(c0.TotaleDocumenti) as Giacenza
                                    FROM Scatole AS s
                                    INNER JOIN Commesse AS c ON s.IdCommessa = c.IdCommessa
                                    INNER JOIN Piattaforme AS p ON c.IdPiattaforma = p.IdPiattaforma
                                    INNER JOIN Contenitori AS c0 ON s.IdContenitore = c0.IdContenitore
                                    INNER JOIN CentriLav AS c1 ON s.IdCentroGiacenza = c1.IdCentroLavorazione
                                    WHERE s.DataNormalizzazione <= {0} and c.Attiva = 1 and s.IdStato = 1 and IdCentroGiacenza = {1}  
                                    group by p.Piattaforma,c1.CentroLavDesc
                                    order by c1.CentroLavDesc,p.Piattaforma";

                            using (SqlCommand cmd = new SqlCommand(sql))
                            {

                                cmd.Connection = con;
                                cmd.CommandTimeout = 120;
                                cmd.Parameters.Clear();

                                string[] myParams = { fromDate, CentroID.ToString() };

                                LstGiacenzaView = await _context.Set<GiacenzaView>().FromSqlRaw(sql, myParams).ToListAsync();
                            }
                        }
                    }


                    TotaleGiacenza = LstGiacenzaView.Sum(x => x.Giacenza);

                    if (LstGiacenzaView.Count == 0)
                    {
                        Message = "Not results found";
                    }

                    return Partial("_RiepilogoGiacenza", this);

                }
                else
                {
                    return Page();
                }

            }
            catch (SqlException ex)
            {
                if (ex.Number == -2)
                {
                    _logger.LogError(ex.Message);
                    ErrorMessage = ex.Message;
                }
                return Partial("_RiepilogoGiacenza", this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ErrorMessage = ex.Message;
                return Partial("_RiepilogoGiacenza", this);
            }




        }


        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;

            if (ModelState.IsValid)
            {
                //raggruppati per piattaforma
                if(IdTipo == 1)
                {
                    return await ReportPiattaformaAsync();
                }

                //Dettaglio
                if (IdTipo == 2)
                {
                    return await ReportDettaglioAsync();
                }
            }
            else
            {
                return Page();
            }

            return Page();

        }




    }
}
