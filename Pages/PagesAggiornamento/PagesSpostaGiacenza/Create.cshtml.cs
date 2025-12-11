using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GiacenzaSorterRm.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Authorization;


namespace GiacenzaSorterRm.Pages.PagesAggiornamento.PagesMacero
{
    [Authorize(Policy = "NormalizzazioneRequirements")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
            ScatoleModel = new ScatoleModel();
        }

        public ScatoleModel ScatoleModel { get; set; }

        public List<Scatole> ScatoleLst { get; set; } = new List<Scatole>();

        [BindProperty(SupportsGet = true)]
        public ScatolaDto Scatole { get; set; } = new ScatolaDto();

        public SelectList CentriSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public async Task<IActionResult> OnGet()
        {
            ViewData["Title"] = "Sposta Giacenza";

            CentriSL = new SelectList(_context.CentriLavs.Where(x => x.IdCentroLavorazione != 5), "IdCentroLavorazione", "CentroLavDesc");

            Scatole = new ScatolaDto
            {
                DataCambioGiacenza = DateTime.Now.Date
            };

            ScatoleModel = new ScatoleModel
            {
                ScatoleLst = await GetListScatole(Scatole.DataCambioGiacenza)
            };

            return Page();
        }

        public async Task<List<Scatole>> GetListScatole(DateTime? dataLavorazione)
        {
            if (!dataLavorazione.HasValue)
            {
                return new List<Scatole>();
            }

            ScatoleLst = await _context.Scatoles
                .Include(s => s.IdCommessaNavigation)
                .Include(s => s.IdContenitoreNavigation)
                .Include(s => s.IdStatoNavigation)
                .Include(s => s.IdTipoNormalizzazioneNavigation)
                .Include(s => s.IdCentroGiacenzaNavigation)
                .Include(s => s.IdCentroNormalizzazioneNavigation)
                .Include(s => s.IdTipologiaNavigation)
                .Where(x => x.DataCambioGiacenza == dataLavorazione.Value.Date)
                .OrderBy(x => x.IdScatola)
                .ToListAsync();

            return ScatoleLst;
        }

        public async Task<IActionResult> OnPostAggiornaScatolaAsync()
        {
            if (!ModelState.IsValid)
            {
                CentriSL = new SelectList(_context.CentriLavs.Where(x => x.IdCentroLavorazione != 5), "IdCentroLavorazione", "CentroLavDesc");

                Scatole = new ScatolaDto
                {
                    DataCambioGiacenza = DateTime.Now.Date
                };

                ScatoleModel = new ScatoleModel
                {
                    LastScatola = Scatole.Scatola,
                    ScatoleLst = await GetListScatole(Scatole.DataCambioGiacenza)
                };

                return Partial("_RiepilogoScatoleGiacenza", ScatoleModel);
            }
            else
            {
                try
                {
                    ScatoleModel = new ScatoleModel();

                    var ScatolaGiacenza = await _context.Scatoles.Where(x => x.Scatola == Scatole.Scatola).FirstOrDefaultAsync();

                    if (ScatolaGiacenza != null)
                    {
                        // data Sorter già presente
                        if (ScatolaGiacenza.DataSorter != null)
                        {
                            CentriSL = new SelectList(_context.CentriLavs.Where(x => x.IdCentroLavorazione != 5), "IdCentroLavorazione", "CentroLavDesc");

                            Scatole = new ScatolaDto
                            {
                                DataCambioGiacenza = DateTime.Now.Date
                            };

                            ScatoleModel = new ScatoleModel
                            {
                                ScatolaSorterizzata = true,
                                LastScatola = Scatole.Scatola,
                                ScatoleLst = await GetListScatole(Scatole.DataCambioGiacenza)
                            };

                            return Partial("_RiepilogoScatoleGiacenza", ScatoleModel);
                        }

                        // Aggiorno scatola giacenza
                        ScatolaGiacenza.DataCambioGiacenza = Scatole.DataCambioGiacenza;
                        ScatolaGiacenza.OperatoreCambioGiacenza = User.Identity?.Name ?? string.Empty;
                        ScatolaGiacenza.NoteCambioGiacenza = Scatole.NoteCambioGiacenza;
                        ScatolaGiacenza.IdCentroGiacenza = Scatole.IdCentroGiacenza;

                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Update Cambio Giacenza scatola normalizzata: {Scatola} - operatore: {Operatore}", 
                            Scatole.Scatola, User.Identity?.Name ?? "Unknown");

                        ScatoleModel.LastScatola = Scatole.Scatola;
                        ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataCambioGiacenza);
                        return Partial("_RiepilogoScatoleGiacenza", ScatoleModel);
                    }
                    else
                    {
                        // Scatola non presente in DB
                        ScatoleModel.ScatolaNonPresenteInDB = true;
                        ScatoleModel.LastScatola = Scatole.Scatola;
                        ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataCambioGiacenza);
                        return Partial("_RiepilogoScatoleGiacenza", ScatoleModel);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Cambio Giacenza for scatola: {Scatola}", Scatole.Scatola);
                    return Partial("_RiepilogoScatoleGiacenza", ScatoleModel);
                }
            }
        }
    }
}
