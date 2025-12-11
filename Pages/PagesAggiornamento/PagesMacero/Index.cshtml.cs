using GiacenzaSorterRm.Models.Database;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace GiacenzaSorterRm.Pages.PagesAggiornamento.PagesMacero
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

        public string Ruolo { get; set; } = string.Empty;
        
        public string Utente { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;

        public SelectList CommesseSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Commessa is required")]
        [Required]
        public int? IdCommessa { get; set; }

        public List<MaceroView> LstMaceroView { get; set; } = new List<MaceroView>();

        public async Task<IActionResult> OnGet()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;
            var sel = await _context.Commesses.ToListAsync();
            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");

            return Page();
        }

        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var startDate = StartDate.Date;
                        var endDate = EndDate.Date;

                        _logger.LogInformation("Query con StartDate={StartDate}, EndDate={EndDate}, IdCommessa={IdCommessa}", 
                            startDate, endDate, IdCommessa);

                        var queryResults = await (from s in _context.Scatoles
                                                  join c in _context.Commesses on s.IdCommessa equals c.IdCommessa
                                                  join p in _context.Piattaformes on c.IdPiattaforma equals p.IdPiattaforma into piattaformaJoin
                                                  from p in piattaformaJoin.DefaultIfEmpty()
                                                  join c0 in _context.Contenitoris on s.IdContenitore equals c0.IdContenitore
                                                  join s0 in _context.Statis on s.IdStato equals s0.IdStato
                                                  join c1 in _context.CentriLavs on s.IdCentroGiacenza equals c1.IdCentroLavorazione into centroJoin
                                                  from c1 in centroJoin.DefaultIfEmpty()
                                                  where s.IdStato == 1 && s.IdCommessa == IdCommessa
                                                  select new
                                                  {
                                                      Centro = c1.CentroLavDesc,
                                                      Piattaforma = p.Piattaforma,
                                                      Commessa = c.Commessa,
                                                      TotaleDocumenti = c0.TotaleDocumenti,
                                                      Scatola = s.Scatola,
                                                      DataNormalizzazione = s.DataNormalizzazione
                                                  }).ToListAsync();

                        _logger.LogInformation("Query ha restituito {Count} record prima del filtro date", queryResults.Count);

                        var filteredResults = queryResults
                            .Where(x => x.DataNormalizzazione.Date >= startDate && x.DataNormalizzazione.Date <= endDate)
                            .ToList();

                        _logger.LogInformation("Dopo filtro date: {Count} record", filteredResults.Count);

                        LstMaceroView = filteredResults
                            .GroupBy(x => new
                            {
                                Centro = x.Centro ?? "Non Assegnato",
                                Piattaforma = x.Piattaforma ?? "N/A",
                                x.Commessa
                            })
                            .Select(g => new MaceroView
                            {
                                Centro = g.Key.Centro,
                                Piattaforma = g.Key.Piattaforma,
                                Commessa = g.Key.Commessa,
                                Giacenza_Documenti = g.Sum(x => x.TotaleDocumenti),
                                Numero_Scatole = g.Count()
                            })
                            .OrderBy(x => x.Centro)
                            .ThenBy(x => x.Piattaforma)
                            .ToList();

                        if (LstMaceroView.Count == 0)
                        {
                            Message = "Not results found";
                        }

                        return Partial("_RiepilogoMacero", this);
                    }
                    catch (SqlException ex) when (ex.Number == -2)
                    {
                        _logger.LogError(ex, "Timeout SQL Server durante il caricamento del report macero");
                        LstMaceroView = new List<MaceroView>();
                        Message = "Timeout: Il caricamento del report sta impiegando troppo tempo. Prova a ridurre l'intervallo di date.";
                        return Partial("_RiepilogoMacero", this);
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError(ex, "Operazione annullata per timeout durante il caricamento del report macero");
                        LstMaceroView = new List<MaceroView>();
                        Message = "Timeout: La richiesta è stata annullata. Prova con un intervallo di date più piccolo.";
                        return Partial("_RiepilogoMacero", this);
                    }
                    catch (TimeoutException ex)
                    {
                        _logger.LogError(ex, "Timeout durante il caricamento del report macero");
                        LstMaceroView = new List<MaceroView>();
                        Message = "Timeout: Il caricamento del report ha superato il tempo massimo.";
                        return Partial("_RiepilogoMacero", this);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Errore durante il caricamento del report macero");
                        LstMaceroView = new List<MaceroView>();
                        Message = "Errore durante il caricamento del report: " + ex.Message;
                        return Partial("_RiepilogoMacero", this);
                    }
                }
                else
                {
                    LstMaceroView = new List<MaceroView>();
                    Message = "Dati non validi. Verificare i campi inseriti.";
                    return Partial("_RiepilogoMacero", this);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore generale nel report macero");
                LstMaceroView = new List<MaceroView>();
                Message = "Errore generale: " + ex.Message;
                return Partial("_RiepilogoMacero", this);
            }
        }

        public async Task<IActionResult> OnPostMacera()
        {
            try
            {
                Utente = User.Identity?.Name ?? string.Empty;

                var startDate = StartDate.Date;
                var endDate = EndDate.Date;

                var scatoleDaMacerare = await _context.Scatoles
                    .Where(s => s.IdCommessa == IdCommessa && s.IdStato == 1)
                    .ToListAsync();

                var scatoleFiltrate = scatoleDaMacerare
                    .Where(s => s.DataNormalizzazione.Date >= startDate && s.DataNormalizzazione.Date <= endDate)
                    .ToList();

                int rowsAffected = scatoleFiltrate.Count;

                if (rowsAffected > 0)
                {
                    foreach (var scatola in scatoleFiltrate)
                    {
                        scatola.IdStato = 3;
                        scatola.OperatoreMacero = Utente;
                        scatola.DataMacero = DateTime.Now.Date;
                    }

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Macerate {RowsAffected} scatole dal {StartDate} al {EndDate} - da operatore: {Operatore}", 
                        rowsAffected, StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd"), User.Identity?.Name ?? "Unknown");

                    Message = $"Operazione completata con successo! {rowsAffected} scatole sono state macerate.";
                }
                else
                {
                    _logger.LogWarning("Nessuna scatola trovata per macero - IdCommessa: {IdCommessa}, Date: {StartDate} - {EndDate}", 
                        IdCommessa, StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd"));
                    Message = "Nessuna scatola trovata per il macero. Potrebbero essere già state macerate.";
                }

                LstMaceroView = new List<MaceroView>();
                return Partial("_RiepilogoMacero", this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'operazione di macero");
                LstMaceroView = new List<MaceroView>();
                Message = "Errore durante l'operazione di macero: " + ex.Message;
                return Partial("_RiepilogoMacero", this);
            }
        }
    }
}
