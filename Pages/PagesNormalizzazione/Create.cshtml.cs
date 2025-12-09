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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace GiacenzaSorterRm.Pages.PagesNormalizzazione
{
    [Authorize(Policy = "NormalizzazioneRequirements")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly IConfiguration _configuration;

        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public SelectList CommesseSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        
        public SelectList TipologieSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        
        public SelectList ContenitoriSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        
        public SelectList TipoNormSL { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        
        public List<Scatole> ScatoleLst { get; set; } = new List<Scatole>();
        
        public ScatoleModel ScatoleModel { get; set; } = new ScatoleModel();

        [BindProperty(SupportsGet = true)]
        public Scatole Scatole { get; set; } = new Scatole();


        public async Task<IActionResult> OnGet()
        {
            ViewData["Title"] = "Normalizzazione";
            await InitializePageAsync();

            ScatoleModel = new ScatoleModel
            {
                ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostInsertScatolaAsync()
        {
            if (!ModelState.IsValid)
            {
                await InitializePageAsync();
                ScatoleModel = new ScatoleModel
                {
                    LastScatola = Scatole.Scatola,
                    ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione)
                };

                return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
            }
            else
            {
                try
                {
                    ScatoleModel = new ScatoleModel();

                    var res = await ControllaNomeScatolaMondoAsync(Scatole.Scatola);

                    if (!res)
                    {
                        ScatoleModel.ScatolaNonPresenteMondo = true;
                        ScatoleModel.IsNotConforme = true;
                        ScatoleModel.LastScatola = Scatole.Scatola;
                        ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione);
                        return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                    }

                    if (ControllaNomeScatola(Scatole.Scatola))
                    {
                        ScatoleModel.IsNotConforme = true;
                        ScatoleModel.LastScatola = Scatole.Scatola;
                        ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione);
                        return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                    }

                    if (!ScatolaExists(Scatole.Scatola))
                    {
                        Scatole.OperatoreNormalizzazione = User.Identity?.Name ?? string.Empty;
                        Scatole.IdStato = 1;
                        Scatole.IdCentroNormalizzazione = CentroAppartenenza.SetCentroByUser(User);
                        Scatole.IdCentroGiacenza = CentroAppartenenza.SetCentroByUser(User);

                        _context.Scatoles.Add(Scatole);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Insert scatola normalizzata: {Scatola} - operatore: {Operatore}", 
                            Scatole.Scatola, User.Identity?.Name ?? "Unknown");
                    }
                    else
                    {
                        ScatoleModel.IsNormalizzata = true;
                    }

                    ScatoleModel.LastScatola = Scatole.Scatola;
                    ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione);
                    return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inserting scatola");
                    return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                }
            }
        }

        private bool ControllaNomeScatola(string nomeScatola)
        {
            if (!string.IsNullOrEmpty(nomeScatola) && (nomeScatola.Length >= 8))
            {
                var part = nomeScatola.Substring(0, 8);
                var count = Regex.Matches(nomeScatola, part).Count;

                if (count > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> ControllaNomeScatolaMondoAsync(string nomeScatola)
        {
            if (string.IsNullOrWhiteSpace(nomeScatola))
            {
                _logger.LogWarning("ControllaNomeScatolaMondo: nome scatola vuoto o null");
                return false;
            }

            string? connectionString = _configuration.GetConnectionString("MondoConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError("MondoConnection string non configurata");
                throw new InvalidOperationException("MondoConnection non configurata. Verificare appsettings e User Secrets.");
            }

            bool res = false;
            string sql = @"SELECT COD_STAMPA FROM MND_SCATOLE_STAMPATE_LISTA WHERE COD_STAMPA = @scatola";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@scatola", nomeScatola);
                        cmd.CommandTimeout = 30;

                        await connection.OpenAsync();

                        var scatolaMondo = await cmd.ExecuteScalarAsync();

                        if (scatolaMondo != null)
                        {
                            res = scatolaMondo.ToString()?.Equals(nomeScatola, StringComparison.OrdinalIgnoreCase) ?? false;

                            if (res)
                            {
                                _logger.LogDebug("Scatola {Scatola} trovata su Mondo", nomeScatola);
                            }
                            else
                            {
                                _logger.LogWarning("Scatola {Scatola} trovata su Mondo ma con nome diverso: {ScatolaMondo}", 
                                    nomeScatola, scatolaMondo);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Scatola {Scatola} NON trovata su Mondo", nomeScatola);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Errore SQL durante verifica scatola {Scatola} su Mondo", nomeScatola);
                throw new InvalidOperationException($"Errore connessione database Mondo: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore generico durante verifica scatola {Scatola} su Mondo", nomeScatola);
                throw;
            }

            return res;
        }

        private bool ScatolaExists(string scatola)
        {
            return _context.Scatoles.Any(e => e.Scatola == scatola);
        }

        public async Task InitializePageAsync()
        {
            Scatole = new Scatole
            {
                DataNormalizzazione = DateTime.Now
            };

            var sel = await _context.Commesses
                .Where(x => x.Attiva == true)
                .OrderBy(x => x.Commessa)
                .ToListAsync();

            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");
            TipologieSL = new SelectList(_context.Tipologies, "IdTipologia", "Tipologia");
            ContenitoriSL = new SelectList(_context.Contenitoris, "IdContenitore", "Contenitore");
            TipoNormSL = new SelectList(_context.TipiNormalizzaziones, "IdTipoNormalizzazione", "TipoNormalizzazione");
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
                    .Where(x => x.DataNormalizzazione == dataLavorazione.Value.Date)
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
                    .Where(x => x.DataNormalizzazione == dataLavorazione.Value.Date && x.IdCentroNormalizzazione == idCentro)
                    .OrderBy(x => x.IdScatola)
                    .ToListAsync();
            }

            return ScatoleLst;
        }

        public async Task<JsonResult> OnGetFindIdCommessaAsync(string bancale)
        {
            string jsondata = string.Empty;

            Bancali? pallet = await _context.Bancalis.Where(x => x.Bancale == bancale).FirstOrDefaultAsync();
            
            if (pallet != null)
            {
                var idCommessa = pallet.IdCommessa;
                var dataAccettazione = pallet.DataAccettazioneBancale;
                List<object> lst = new List<object> { idCommessa, dataAccettazione };
                jsondata = JsonConvert.SerializeObject(lst);
            }

            return new JsonResult(jsondata);
        }

        public async Task<JsonResult> OnGetAssociazioneTipologiaAsync(int idCommessa)
        {
            var lstTipologie = await _context.CommessaTipologiaContenitores
                .Where(x => x.IdCommessa == idCommessa && x.Attiva == true)
                .OrderBy(x => x.IdRiepilogo)
                .Select(x => x.IdTipologia)
                .ToListAsync();

            var SelectTipologie = await _context.Tipologies
                .Where(x => lstTipologie.Contains(x.IdTipologia))
                .Select(a => new SelectListItem
                {
                    Value = a.IdTipologia.ToString(),
                    Text = a.Tipologia
                })
                .AsNoTracking()
                .OrderBy(x => x.Text)
                .ToListAsync();

            string jsondata = JsonConvert.SerializeObject(SelectTipologie);
            return new JsonResult(jsondata);
        }

        public async Task<JsonResult> OnGetAssociazioneContenitoreAsync(int idCommessa, int idTipologia)
        {
            var lstContenitori = await _context.CommessaTipologiaContenitores
                .Where(x => x.IdCommessa == idCommessa && x.IdTipologia == idTipologia && x.Attiva == true)
                .Select(x => x.IdContenitore)
                .ToListAsync();

            var SelectContenitore = await _context.Contenitoris
                .Where(x => lstContenitori.Contains(x.IdContenitore))
                .Select(a => new SelectListItem
                {
                    Value = a.IdContenitore.ToString(),
                    Text = a.Contenitore
                })
                .AsNoTracking()
                .ToListAsync();

            string jsondata = JsonConvert.SerializeObject(SelectContenitore);
            return new JsonResult(jsondata);
        }

        private bool IsDataScatolaBeforeDataBancale(Scatole scatola, Bancali bancale)
        {
            int compare = DateTime.Compare(scatola.DataNormalizzazione, bancale.DataAccettazioneBancale);
            return compare < 0;
        }
    }
}
