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
using System.Data.SqlClient;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;

namespace GiacenzaSorterRm.Pages.PagesNormalizzazione
{

    [Authorize(Policy = "NormalizzazioneRequirements")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        public SelectList CommesseSL { get; set; } 
        public SelectList TipologieSL { get; set; }
        public SelectList ContenitoriSL { get; set; }
        public SelectList TipoNormSL { get; set; }
        public List<Scatole> ScatoleLst { get; set; }
        public ScatoleModel ScatoleModel { get; set; }


        [BindProperty(SupportsGet = true)]
        public Scatole Scatole { get; set; }


        ///[BindProperty(SupportsGet = true)]
        //public Bancali Bancali { get; set; }


        public async Task<IActionResult> OnGet()
        {

            ViewData["Title"] =  "Normalizzazione";
            await InitializePageAsync();

            ScatoleModel = new ScatoleModel
            {
                ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione)
            };

            return Page();
        }



        public async Task<IActionResult> OnPostInsertScatolaAsync()
        {
            //var pallet = await _context.Bancalis.Where(x => x.Bancale == Bancali.Bancale).FirstOrDefaultAsync();

            //if (IsDataScatolaBeforeDataBancale(Scatole, pallet))
            //{
            //    ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione);
            //    ModelState.AddModelError("Scatole.DataNormalizzazione", "Attenzione! Data inserita antecedente accettazione bancale");
            //}

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
                        //ScatoleModel.IsNotConforme = true;

                        ScatoleModel.LastScatola = Scatole.Scatola;
                        ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione);
                        return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                    }


                    //if (ControllaNomeScatola(Scatole.Scatola))
                    //{
                    //    ScatoleModel.IsNotConforme = true;

                    //    ScatoleModel.LastScatola = Scatole.Scatola;
                    //    ScatoleModel.ScatoleLst = await GetListScatole(Scatole.DataNormalizzazione);
                    //    return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                    //}



                    if (!ScatolaExists(Scatole.Scatola))
                    {
                        Scatole.OperatoreNormalizzazione = User.Identity.Name;
                        Scatole.IdStato = 1;
                        Scatole.IdCentroNormalizzazione = CentroAppartenenza.SetCentroByUser(User);
                        Scatole.IdCentroGiacenza = CentroAppartenenza.SetCentroByUser(User);
                        //Scatole.IdBancale = pallet.IdBancale;

                        _context.Scatoles.Add(Scatole);

                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Insert scatola normalizzata : " + Scatole.Scatola + " - operatore : " + User.Identity.Name);
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
                    _logger.LogError(ex.Message);
                    return Partial("_RiepilogoNormalizzateInserite", ScatoleModel);
                }
            }
        }




        /// <summary>
        /// Controlla che il nome scatola non sia ripetuto più volte a causa di un inserimento troppop veloce
        /// conta le occorrenze della parola formata dalle prime 8 lettere all'interno della stringa di input
        /// </summary>
        /// <param name="scatola"></param>
        /// <returns></returns>
        private bool ControllaNomeScatola (string nomeScatola)
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




        /// <summary>
        /// Controlla che il nome scatola esiste su Mondo
        /// </summary>
        /// <param name="scatola"></param>
        /// <returns></returns>
        private async Task<bool> ControllaNomeScatolaMondoAsync(string nomeScatola)
        {
            bool res = false;
            string sql = @"SELECT COD_STAMPA FROM MND_SCATOLE_STAMPATE_LISTA where COD_STAMPA = @scatola";

            using (SqlConnection connection = new SqlConnection(MyConnections.CnxnMondo))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@scatola", nomeScatola);
             
                connection.Open();
                var scatolaMondo = await cmd.ExecuteScalarAsync();
                if (scatolaMondo != null)
                {
                    if (scatolaMondo.ToString() == nomeScatola)
                    {
                        res = true;
                    }
                    else
                    {
                        res = false;
                    }
                }
            }

            return res;
        }


        private bool ScatolaExists(string scatola)
        {
            return _context.Scatoles.Any(e => e.Scatola == scatola);
        }


        public async Task InitializePageAsync()
        {
            int centroID = CentroAppartenenza.SetCentroByUser(User);

            Scatole = new Scatole
            {
                DataNormalizzazione = DateTime.Now
            };

            var sel = await _context.Commesses.Where(x=> x.Attiva == true).OrderBy(x=> x.Commessa).ToListAsync();

            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");
            
            TipologieSL = new SelectList(_context.Tipologies, "IdTipologia", "Tipologia");
            ContenitoriSL = new SelectList(_context.Contenitoris, "IdContenitore", "Contenitore");
            TipoNormSL = new SelectList(_context.TipiNormalizzaziones, "IdTipoNormalizzazione", "TipoNormalizzazione");
        }


        public async Task<List<Scatole>> GetListScatole(DateTime? dataLavorazione)
        {
            
            if (User.IsInRole("ADMIN"))
            {
                ScatoleLst = await _context.Scatoles
                                  .Include(s => s.IdCommessaNavigation)
                                  .Include(s => s.IdContenitoreNavigation)
                                  .Include(s => s.IdStatoNavigation)
                                  .Include(s => s.IdTipoNormalizzazioneNavigation)
                                  .Include(s => s.IdTipologiaNavigation).Where(x => x.DataNormalizzazione == dataLavorazione.Value.Date ).OrderBy(x => x.IdScatola).ToListAsync();
            }
            else
            {
                int idCentro = CentroAppartenenza.SetCentroByUser(User);

                ScatoleLst = await _context.Scatoles
                          .Include(s => s.IdCommessaNavigation)
                          .Include(s => s.IdContenitoreNavigation)
                          .Include(s => s.IdStatoNavigation)
                          .Include(s => s.IdTipoNormalizzazioneNavigation)
                          .Include(s => s.IdTipologiaNavigation).Where(x => x.DataNormalizzazione == dataLavorazione.Value.Date && x.IdCentroNormalizzazione == idCentro).OrderBy(x => x.IdScatola).ToListAsync();

            }

            return ScatoleLst;

        }


        public async Task<JsonResult> OnGetFindIdCommessaAsync(string bancale)
        {
            //var idCommessa = await _context.Bancalis.Where(x => x.Bancale == bancale ).Select(x => x.IdCommessa).FirstOrDefaultAsync();
            //string jsondata = JsonConvert.SerializeObject(idCommessa);
            //return new JsonResult(jsondata);
            string jsondata = "";

            var pallet = await _context.Bancalis.Where(x => x.Bancale == bancale).FirstOrDefaultAsync();
            if(pallet != null)
            {
                var idCommessa = pallet.IdCommessa;
                var dataAccettazione = pallet.DataAccettazioneBancale;
                List<object> lst = new List<object>();
                lst.Add(idCommessa);
                lst.Add(dataAccettazione);
                jsondata = JsonConvert.SerializeObject(lst);
            }
            
            
            return new JsonResult(jsondata);

        }


        public async Task<JsonResult> OnGetAssociazioneTipologiaAsync(int idCommessa)
        {
            var lstTipologie = await _context.CommessaTipologiaContenitores.Where(x => x.IdCommessa == idCommessa && x.Attiva == true).OrderBy(x => x.IdRiepilogo).Select(x => x.IdTipologia).ToListAsync();

            var SelectTipologie = await _context.Tipologies.Where(x => lstTipologie.Contains(x.IdTipologia)).Select(a =>
                                        new SelectListItem
                                        {
                                            Value = a.IdTipologia.ToString(),
                                            Text = a.Tipologia
                                        }).AsNoTracking().OrderBy(x=> x.Text).ToListAsync();
            
            string jsondata = JsonConvert.SerializeObject(SelectTipologie);
            
            return new JsonResult(jsondata);
        }


        public async Task<JsonResult> OnGetAssociazioneContenitoreAsync(int idCommessa, int idTipologia)
        {
            var lstContenitori = await _context.CommessaTipologiaContenitores.Where(x => x.IdCommessa == idCommessa && x.IdTipologia==idTipologia && x.Attiva == true).Select(x => x.IdContenitore).ToListAsync();

            var SelectContenitore = await _context.Contenitoris.Where(x => lstContenitori.Contains(x.IdContenitore)).Select(a =>
                                        new SelectListItem
                                        {
                                            Value = a.IdContenitore.ToString(),
                                            Text = a.Contenitore
                                        }).AsNoTracking().ToListAsync();

            string jsondata = JsonConvert.SerializeObject(SelectContenitore);

            return new JsonResult(jsondata);

        }



        /// <summary>
        /// data scatola non può essere precedente a data bancale
        /// </summary>
        /// <param name="scatola"></param>
        /// <returns></returns>
        private bool IsDataScatolaBeforeDataBancale(Scatole scatola, Bancali bancale)
        {
            bool res ;
            int compare = DateTime.Compare(scatola.DataNormalizzazione, bancale.DataAccettazioneBancale);
            if(compare < 0)
            {
                res = true;
            }
            else
            {
                res = false;
            }
            return res;
        }






    }




}
