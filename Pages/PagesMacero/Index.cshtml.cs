using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace GiacenzaSorterRm.Pages.PagesMacero
{
    
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }


        public string Ruolo { get; set; }
        public string Utente { get; set; }
        public string Message { get; set; } = string.Empty;

        public SelectList CommesseSL { get; set; }



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


        public List<MaceroView> LstMaceroView { get; set; }



        public async Task<IActionResult> OnGet()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;
            var sel = await _context.Commesses.ToListAsync();
            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");

            return Page();
        }


        public async Task<IActionResult> OnPostReportAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;

            try
            {
                                                                       
                if (ModelState.IsValid)
                {
                    LstMaceroView = new List<MaceroView>();
                    string sql = "";
                    string constr = _context.Database.GetDbConnection().ConnectionString;

                    try
                    {
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            string fromDate = StartDate.Date.ToString("yyyyMMdd");
                            string toDate = EndDate.Date.ToString("yyyyMMdd");


                            //trova le scatole che sono in stato normalizzato tramite idCommessa e data normalizzazione
                            sql = @"SELECT c1.CentroLavDesc as Centro,p.Piattaforma,c.Commessa,sum(c0.TotaleDocumenti) as Giacenza_Documenti, count(s.Scatola) as Numero_Scatole
                                    FROM Scatole AS s
                                    INNER JOIN Commesse AS c ON s.IdCommessa = c.IdCommessa
                                    INNER JOIN Piattaforme AS p ON c.IdPiattaforma = p.IdPiattaforma
                                    INNER JOIN Contenitori AS c0 ON s.IdContenitore = c0.IdContenitore
                                    INNER JOIN Stati AS s0 ON s.IdStato = s0.IdStato
                                    INNER JOIN CentriLav AS c1 ON s.IdCentroGiacenza = c1.IdCentroLavorazione
                                    INNER JOIN Tipologie AS t ON s.IdTipologia = t.IdTipologia
                                    WHERE s.IdStato = 1 and convert(date,s.DataNormalizzazione) >= {0} and convert(date,s.DataNormalizzazione) <= {1} and s.IdCommessa = {2} 
                                    group by p.Piattaforma,c1.CentroLavDesc,c.Commessa
                                    order by c1.CentroLavDesc,p.Piattaforma";


                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = con;
                                cmd.CommandTimeout = 120;
                                cmd.Parameters.Clear();

                                string[] myParams = { fromDate,toDate,IdCommessa.ToString() };

                                LstMaceroView = await _context.Set<MaceroView>().FromSqlRaw(sql, myParams).ToListAsync();

                                if (LstMaceroView.Count == 0)
                                {
                                    Message = "Not results found";
                                }

                                return Partial("_RiepilogoMacero", this);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        return Page();
                    }
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


        public async Task<IActionResult> OnPostMacera()
        {
            try
            {
                Utente = User.Identity.Name;

                LstMaceroView = new List<MaceroView>();
                string sql = "";
                string constr = _context.Database.GetDbConnection().ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    string fromDate = StartDate.Date.ToString("yyyyMMdd");
                    string toDate = EndDate.Date.ToString("yyyyMMdd");

                    //trova le scatole che sono in stato normalizzato tramite idCommessa e data normalizzazione
                    sql = @"update scatole set IdStato = 3, OperatoreMacero=@operatoreMacero, DataMacero=@dataMacero where DataNormalizzazione >= @fromDate and DataNormalizzazione <= @toDate and IdCommessa = @IdCommessa and IdStato = 1  ";

                    using (SqlCommand cmd = new SqlCommand(sql))
                    {

                        cmd.Connection = con;
                        cmd.Parameters.Clear();

                        SqlParameter param1 = new SqlParameter
                        {
                            ParameterName = "@fromDate",
                            Value = StartDate.Date
                        };
                        cmd.Parameters.Add(param1);

                        SqlParameter param2 = new SqlParameter
                        {
                            ParameterName = "@toDate",
                            Value = EndDate.Date
                        };
                        cmd.Parameters.Add(param2);

                        SqlParameter param3 = new SqlParameter
                        {
                            ParameterName = "@IdCommessa",
                            Value = IdCommessa
                        };

                        cmd.Parameters.Add(param3);


                        SqlParameter param4 = new SqlParameter
                        {
                            ParameterName = "@operatoreMacero",
                            Value = Utente
                        };

                        cmd.Parameters.Add(param4);

                        SqlParameter param5 = new SqlParameter
                        {
                            ParameterName = "@dataMacero",
                            Value = DateTime.Now.Date
                        };

                        cmd.Parameters.Add(param5);


                        await cmd.ExecuteNonQueryAsync();

                        _logger.LogInformation("Macerate scatole dal "+ StartDate + " al " + EndDate +  " - da operatore : " + User.Identity.Name);

                        Message = "Scatole Macerate";

                        return Partial("_RiepilogoMacero", this);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Page();
            }
        }

    }
}
