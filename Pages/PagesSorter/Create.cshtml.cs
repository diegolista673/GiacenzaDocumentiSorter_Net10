using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GiacenzaSorterRm.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Authorization;

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
                ScatoleLst = await GetListScatole(Scatole.DataSorter),
                Default = true
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ScatoleModel = new ScatoleModel
                {
                    LastScatola = Scatole.Scatola,
                    ScatoleLst = await GetListScatole(Scatole.DataSorter)
                };
                return Partial("_RiepilogoScatoleSorter", ScatoleModel);
            }

            try
            {
                Scatole? ScatolaGiacenza = await _context.Scatoles
                    .Where(x => x.Scatola == Scatole.Scatola)
                    .FirstOrDefaultAsync();

                if (ScatolaGiacenza != null)
                {
                    // se scatola è già stata sorterizzata
                    if (ScatolaGiacenza.DataSorter != null)
                    {
                        ScatoleModel = new ScatoleModel
                        {
                            LastScatola = Scatole.Scatola,
                            ScatoleLst = await GetListScatole(Scatole.DataSorter),
                            IsNormalizzata = true,
                            IsSorterizzata = true,
                            Default = false
                        };
                    }
                    else
                    {
                        Scatole? scatola = await _context.Scatoles.SingleOrDefaultAsync(b => b.Scatola == Scatole.Scatola);
                        
                        if (scatola != null)
                        {
                            scatola.DataSorter = Scatole.DataSorter;
                            scatola.IdCentroSorterizzazione = CentroAppartenenza.SetCentroByUser(User);
                            scatola.IdStato = 2;
                            scatola.OperatoreSorter = User.Identity?.Name ?? string.Empty;
                            scatola.Note = Scatole.Note;

                            await _context.SaveChangesAsync();

                            _logger.LogInformation("Inserimento scatola sorterizzata: {Scatola} - operatore: {Operatore}", 
                                Scatole.Scatola, User.Identity?.Name ?? "Unknown");

                            ScatoleModel = new ScatoleModel
                            {
                                LastScatola = Scatole.Scatola,
                                ScatoleLst = await GetListScatole(Scatole.DataSorter),
                                Default = true
                            };
                        }
                    }
                }
                else
                {
                    // se scatola non esiste chiede di normalizzarla prima
                    ScatoleModel = new ScatoleModel
                    {
                        LastScatola = Scatole.Scatola,
                        ScatoleLst = await GetListScatole(Scatole.DataSorter),
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

        public async Task<List<Scatole>> GetListScatole(DateTime? dataLavorazione)
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
