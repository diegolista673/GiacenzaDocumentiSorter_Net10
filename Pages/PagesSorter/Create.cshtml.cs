using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models;
using GiacenzaSorterRm.Models.Database;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Pages.PagesSorter
{
    [Authorize(Policy = "SorterRequirements")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;

        public List<Scatole> ScatoleLst { get; set; } = new List<Scatole>();

        public ScatoleModel ScatoleModel { get; set; } = new ScatoleModel();

        public string Message { get; set; } = string.Empty;

        [BindProperty]
        public Scatole Scatole { get; set; } = new Scatole();

        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            ViewData["Title"] = "Sorter";
            InitializePage();
            ScatoleModel = new ScatoleModel
            {
                ScatoleLst = await GetListScatoleAsync(Scatole.DataSorter),
                Default = true
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Scatole.Note");
            ModelState.Remove("Scatole.OperatoreMacero");
            ModelState.Remove("Scatole.OperatoreSorter");
            ModelState.Remove("Scatole.IdStatoNavigation");
            ModelState.Remove("Scatole.NoteCambioGiacenza");
            ModelState.Remove("Scatole.IdCommessaNavigation");
            ModelState.Remove("Scatole.IdTipologiaNavigation");
            ModelState.Remove("Scatole.IdTipoNormalizzazione");
            ModelState.Remove("Scatole.IdContenitoreNavigation");
            ModelState.Remove("Scatole.OperatoreCambioGiacenza");
            ModelState.Remove("Scatole.IdCentroGiacenzaNavigation");
            ModelState.Remove("Scatole.IdTipoNormalizzazioneNavigation");
            ModelState.Remove("Scatole.IdCentroNormalizzazioneNavigation");
            ModelState.Remove("Scatole.IdCentroSorterizzazioneNavigation");
            ModelState.Remove("Scatole.OperatoreNormalizzazione");

            if (!ModelState.IsValid)
            {
                ScatoleModel = new ScatoleModel
                {
                    LastScatola = Scatole.Scatola,
                    ScatoleLst = await GetListScatoleAsync(Scatole.DataSorter)
                };

                _logger.LogWarning("Model state is invalid for sorter scatola: {Scatola}", Scatole.Scatola);
                return Partial("_RiepilogoScatoleSorter", ScatoleModel);
            }

            try
            {
                
                var scatolaExists = await _context.Scatoles
                    .Where(x => x.Scatola == Scatole.Scatola)
                    .FirstOrDefaultAsync();

                if (scatolaExists != null)
                {
                    // se scatola è già stata sorterizzata
                    if (scatolaExists.DataSorter != null)
                    {
                        _logger.LogInformation("Scatola già sorterizzata: {Scatola} - operatore: {Operatore}",
                            Scatole.Scatola, User.Identity?.Name ?? "Unknown");

                        ScatoleModel = new ScatoleModel
                        {
                            LastScatola = Scatole.Scatola,
                            ScatoleLst = await GetListScatoleAsync(Scatole.DataSorter),
                            IsNormalizzata = true,
                            IsSorterizzata = true,
                            Default = false
                        };
                    }
                    else
                    {
                        scatolaExists.DataSorter = Scatole.DataSorter;
                        scatolaExists.IdCentroSorterizzazione = CentroAppartenenza.SetCentroByUser(User);
                        scatolaExists.IdStato = 2;
                        scatolaExists.OperatoreSorter = User.Identity?.Name ?? string.Empty;
                        scatolaExists.Note = Scatole.Note;

                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Inserimento scatola sorterizzata: {Scatola} - operatore: {Operatore}", 
                            Scatole.Scatola, User.Identity?.Name ?? "Unknown");

                        ScatoleModel = new ScatoleModel
                        {
                            LastScatola = Scatole.Scatola,
                            ScatoleLst = await GetListScatoleAsync(Scatole.DataSorter),
                            Default = true
                        };

                    }
                }
                else
                {
                    // se scatola non esiste chiede di normalizzarla prima
                    _logger.LogWarning("Scatola non esistente, normalizzazione richiesta: {Scatola} - operatore: {Operatore}", 
                        Scatole.Scatola, User.Identity?.Name ?? "Unknown");

                    ScatoleModel = new ScatoleModel
                    {
                        LastScatola = Scatole.Scatola,
                        ScatoleLst = await GetListScatoleAsync(Scatole.DataSorter),
                        IsNormalizzata = false,
                        IsSorterizzata = false,
                        Default = false
                    };
                }

                return Partial("_RiepilogoScatoleSorter", ScatoleModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sorter scatola");
                return Partial("_RiepilogoScatoleSorter", ScatoleModel);
            }
        }

        private bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            TimeSpan now = datetime.TimeOfDay;
            
            if (start < end)
                return start <= now && now <= end;
            
            return !(end < now && now < start);
        }

        public void InitializePage()
        {
            TimeSpan start = new TimeSpan(24, 0, 0);
            TimeSpan end = new TimeSpan(6, 0, 0);
            bool res = TimeBetween(DateTime.Now, start, end);

            DateTime dataSorter = res ? DateTime.Now.AddDays(-1) : DateTime.Now;

            Scatole = new Scatole
            {
                DataSorter = dataSorter
            };
        }

        public async Task<List<Scatole>> GetListScatoleAsync(DateTime? dataLavorazione)
        {
            if (!dataLavorazione.HasValue)
            {
                return new List<Scatole>();
            }

            if (User.IsInRole("ADMIN"))
            {
                ScatoleLst = await _context.Scatoles
                    .Include(s => s.IdCommessaNavigation)
                    .Include(s => s.IdContenitoreNavigation)
                    .Include(s => s.IdStatoNavigation)
                    .Include(s => s.IdTipoNormalizzazioneNavigation)
                    .Include(s => s.IdTipologiaNavigation)
                    .Where(x => x.DataSorter == dataLavorazione.Value.Date)
                    .OrderBy(x => x.IdScatola)
                    .ToListAsync();
            }
            else
            {
                int idCentro = CentroAppartenenza.SetCentroByUser(User);

                ScatoleLst = await _context.Scatoles
                    .Include(s => s.IdCommessaNavigation)
                    .Include(s => s.IdContenitoreNavigation)
                    .Include(s => s.IdStatoNavigation)
                    .Include(s => s.IdTipoNormalizzazioneNavigation)
                    .Include(s => s.IdTipologiaNavigation)
                    .Where(x => x.DataSorter == dataLavorazione.Value.Date && x.IdCentroSorterizzazione == idCentro)
                    .OrderBy(x => x.IdScatola)
                    .ToListAsync();
            }

            return ScatoleLst;
        }
    }
}
