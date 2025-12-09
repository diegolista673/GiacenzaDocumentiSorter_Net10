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

        public DateTime EndDate { get; set; } = DateTime.Now.Date;

        public string Message { get; set; } = string.Empty;
        
        public List<CommesseView> LstCommesseView { get; set; } = new List<CommesseView>();
        
        public int TotaleGiacenza { get; set; }

        public List<SelectListItem> LstCentri { get; set; } = new List<SelectListItem>();

        public string Ruolo { get; set; } = string.Empty;
        
        public string Utente { get; set; } = string.Empty;

        public List<GiacenzaView> LstGiacenzaView { get; set; } = new List<GiacenzaView>();

        [BindProperty]
        [Required(ErrorMessage = "Centro is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Centro is required")]
        public int SelectedCentro { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "IdTipo is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IdTipo is required")]
        public int IdTipo { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

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

        public async Task<IActionResult> ReportDettaglioAsync()
        {
            try
            {
                Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
                Utente = User.Identity?.Name ?? string.Empty;
                int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);

                _context.Database.SetCommandTimeout(120);

                if (ModelState.IsValid)
                {
                    IQueryable<Scatole> lstSC;

                    if (CentroID == 5)
                    {
                        lstSC = _context.Scatoles
                            .Include(s => s.IdCommessaNavigation)
                            .Include(s => s.IdContenitoreNavigation)
                            .Include(s => s.IdStatoNavigation)
                            .Include(s => s.IdCentroGiacenzaNavigation)
                            .Include(s => s.IdTipologiaNavigation)
                            .Where(s => (s.IdStato == 1 || s.DataSorter == EndDate) && s.IdCommessaNavigation.Attiva == true && s.IdStato < 3)
                            .AsNoTracking();
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
                            .AsNoTracking();
                    }

                    LstCommesseView = await (from p in lstSC
                                             group p by new { p.IdCommessaNavigation.Commessa, p.IdTipologiaNavigation.Tipologia, p.IdCentroGiacenzaNavigation.CentroLavDesc, p.IdCommessaNavigation.IdPiattaformaNavigation.Piattaforma } into g
                                             select new CommesseView
                                             {
                                                 Commessa = g.Key.Commessa,
                                                 Tipologia = g.Key.Tipologia,
                                                 TotaleDocumentiNormalizzati = g.Where(x => x.IdStato == 1 && x.DataNormalizzazione == EndDate).Sum(x => x.IdContenitoreNavigation.TotaleDocumenti ?? 0),
                                                 TotaleDocumentiSorterizzati = g.Where(z => z.IdStato == 2 && z.DataSorter == EndDate).Sum(x => x.IdContenitoreNavigation.TotaleDocumenti ?? 0),
                                                 TotaleDocumentiGiacenza = g.Where(z => z.IdStato == 1).Sum(x => x.IdContenitoreNavigation.TotaleDocumenti ?? 0),
                                                 ScatolaNormalizzataOld = g.Where(p => p.IdStato == 1 && p.IdCommessaNavigation.Commessa == g.Key.Commessa && p.IdTipologiaNavigation.Tipologia == g.Key.Tipologia).OrderBy(x => x.DataNormalizzazione).Select(x => x.Scatola).FirstOrDefault(),
                                                 DataScatolaOld = g.Where(p => p.IdStato == 1 && p.IdCommessaNavigation.Commessa == g.Key.Commessa && p.IdTipologiaNavigation.Tipologia == g.Key.Tipologia).Select(x => x.DataNormalizzazione).Min(),
                                                 CentroGiacenza = g.Key.CentroLavDesc,
                                                 Piattaforma = g.Key.Piattaforma
                                             })
                                             .OrderBy(z => z.Commessa)
                                             .ThenBy(z => z.Tipologia)
                                             .ToListAsync();

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
            catch (SqlException ex) when (ex.Number == -2)
            {
                _logger.LogError(ex, "SQL Timeout in ReportDettaglio");
                ErrorMessage = "Query timeout. Please try with a smaller date range.";
                return Partial("_RiepilogoVolumi", this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReportDettaglio");
                ErrorMessage = "An error occurred while generating the report.";
                return Partial("_RiepilogoVolumi", this);
            }
        }

        public async Task<IActionResult> ReportPiattaformaAsync()
        {
            try
            {
                Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
                Utente = User.Identity?.Name ?? string.Empty;
                int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);

                _context.Database.SetCommandTimeout(120);

                if (ModelState.IsValid)
                {
                    LstGiacenzaView = new List<GiacenzaView>();
                    string sql;
                    string? constr = _context.Database.GetDbConnection().ConnectionString;

                    if (string.IsNullOrEmpty(constr))
                    {
                        _logger.LogError("Connection string is null or empty");
                        ErrorMessage = "Database connection error.";
                        return Partial("_RiepilogoGiacenza", this);
                    }

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string fromDate = EndDate.Date.ToString("yyyyMMdd");

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

                            string[] myParams = { fromDate };
                            LstGiacenzaView = await _context.Set<GiacenzaView>().FromSqlRaw(sql, myParams).ToListAsync();
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

                            string[] myParams = { fromDate, CentroID.ToString() };
                            LstGiacenzaView = await _context.Set<GiacenzaView>().FromSqlRaw(sql, myParams).ToListAsync();
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
            catch (SqlException ex) when (ex.Number == -2)
            {
                _logger.LogError(ex, "SQL Timeout in ReportPiattaforma");
                ErrorMessage = "Query timeout. Please try with a smaller date range.";
                return Partial("_RiepilogoGiacenza", this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReportPiattaforma");
                ErrorMessage = "An error occurred while generating the report.";
                return Partial("_RiepilogoGiacenza", this);
            }
        }

        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;

            if (ModelState.IsValid)
            {
                if (IdTipo == 1)
                {
                    return await ReportPiattaformaAsync();
                }

                if (IdTipo == 2)
                {
                    return await ReportDettaglioAsync();
                }
            }

            return Page();
        }
    }
}
