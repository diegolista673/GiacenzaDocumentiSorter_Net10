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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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

        public string Ruolo { get; set; } = string.Empty;
        
        public string Utente { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public string Fase { get; set; } = string.Empty;
        
        public List<SelectListItem> LstCentri { get; set; } = new List<SelectListItem>();
        
        public SelectList CommesseSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        [BindProperty]
        [Required(ErrorMessage = "Centro is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Centro is required")]
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
        public List<ScatolaView> LstScatoleView { get; set; } = new List<ScatolaView>();

        [Required(ErrorMessage = "Fase is required")]
        [BindProperty]
        public string? SelectedFase { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Commessa is required")]
        [Required]
        public int? IdCommessa { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;

            if (User.FindFirst("Azienda")?.Value == "POSTEL")
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
                LstCentri = await _context.CentriLavs
                    .Where(x => x.IdCentroLavorazione == CentroID)
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdCentroLavorazione.ToString(),
                        Text = a.CentroLavDesc
                    })
                    .OrderBy(x => x.Value)
                    .ToListAsync();
            }

            var sel = await _context.Commesses.OrderBy(x => x.Commessa).ToListAsync();
            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");

            return Page();
        }

        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo") ?? string.Empty;
            Utente = User.Identity?.Name ?? string.Empty;

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
                _logger.LogError(ex, "Error generating report");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostElimina()
        {
            try
            {
                var lst = LstScatoleView.Where(x => x.Elimina == true).ToList();

                if (lst.Count > 0)
                {
                    _logger.LogInformation("Inizio eliminazione di {Count} scatole - operatore: {Operatore}", 
                        lst.Count, User.Identity?.Name ?? "Unknown");

                    int centroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);
                    string fase = SelectedFase == "NORMALIZZAZIONE" ? "normalizzazione" : "sorter";

                    foreach (var item in lst)
                    {
                        Scatole? scatola = await _context.Scatoles.FindAsync(item.IdScatola);
                        if (scatola != null)
                        {
                            _context.Scatoles.Remove(scatola);
                            _logger.LogInformation("Delete scatola: {Scatola} - operatore: {Operatore}", 
                                scatola.Scatola, User.Identity?.Name ?? "Unknown");
                        }
                    }

                    await _context.SaveChangesAsync();
                    await SetSearch(StartDate, EndDate, fase, centroID);

                    Fase = SelectedFase == "NORMALIZZAZIONE" ? "Normalizzate" : "Sorterizzate";

                    return Partial("_RiepilogoScatole", this);
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante eliminazione scatole");
                return new EmptyResult();
            }
        }

        public async Task SetSearch(DateTime startDate, DateTime endDate, string flag, int idCentro)
        {
            IQueryable<Scatole> lstScatole = _context.Scatoles.AsQueryable();

            // tutti i centri and tutte le commesse
            if (idCentro == 5 && IdCommessa == 999)
            {
                if (flag == "normalizzazione")
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate);
                }
                else
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate);
                }
            }
            // Per tutti i centri di lavorazione and 1 commessa
            else if (idCentro == 5 && IdCommessa > 0 && IdCommessa < 999)
            {
                if (flag == "normalizzazione")
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate && x.IdCommessa == IdCommessa);
                }
                else
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate && x.IdCommessa == IdCommessa);
                }
            }
            // Per 1 centro di lavorazione and tutte le commesse
            else if (idCentro != 5 && IdCommessa == 999)
            {
                if (flag == "normalizzazione")
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate && x.IdCentroNormalizzazione == idCentro);
                }
                else
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate && x.IdCentroSorterizzazione == idCentro);
                }
            }
            // Per centro di lavorazione e per 1 commessa
            else if (idCentro != 5 && IdCommessa > 0 && IdCommessa < 999)
            {
                if (flag == "normalizzazione")
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataNormalizzazione >= startDate && x.DataNormalizzazione <= endDate && x.IdCentroNormalizzazione == idCentro && x.IdCommessa == IdCommessa);
                }
                else
                {
                    lstScatole = _context.Scatoles.Where(x => x.DataSorter >= startDate && x.DataSorter <= endDate && x.IdCentroSorterizzazione == idCentro && x.IdCommessa == IdCommessa);
                }
            }

            LstScatoleView = await lstScatole
                .Include(s => s.IdCommessaNavigation)
                .Include(s => s.IdContenitoreNavigation)
                .Include(s => s.IdStatoNavigation)
                .Include(s => s.IdTipoNormalizzazioneNavigation)
                .Include(s => s.IdCentroNormalizzazioneNavigation)
                .Include(s => s.IdCentroSorterizzazioneNavigation)
                .Include(s => s.IdTipologiaNavigation)
                .Select(m => new ScatolaView
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
                    TipoProdotto = m.IdTipoNormalizzazioneNavigation.TipoNormalizzazione,
                    TotaleScatola = m.IdContenitoreNavigation.TotaleDocumenti ?? 0,
                    Elimina = false,
                    IdCentroNormalizzazione = m.IdCentroNormalizzazione ?? 0,
                    IdCentroSorterizzazione = m.IdCentroSorterizzazione,
                    CentroNormalizzazione = m.IdCentroNormalizzazioneNavigation != null ? m.IdCentroNormalizzazioneNavigation.CentroLavDesc : string.Empty,
                    CentroSorterizzazione = m.IdCentroSorterizzazioneNavigation != null ? m.IdCentroSorterizzazioneNavigation.CentroLavDesc : string.Empty
                })
                .OrderBy(x => x.IdScatola)
                .ToListAsync();

            if (LstScatoleView.Count == 0)
            {
                Message = "Not results found";
            }
        }
    }
}
