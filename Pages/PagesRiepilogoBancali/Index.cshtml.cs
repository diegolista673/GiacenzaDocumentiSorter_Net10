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

namespace GiacenzaSorterRm.Pages.PagesRiepilogoBancali
{
    
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly DbContext _dbContext;
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
            _dbContext = context as DbContext;
        }
        public List<SelectListItem> LstCentri { get; set; }


        [BindProperty]
        public int SelectedCentro { get; set; }

        public string Ruolo { get; set; }
        public string Utente { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;


        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public string Message { get; set; }

        public IList<Bancali> Bancali { get;set; }

        [BindProperty]
        public List<BancaleView> LstBancaleView { get; set; }


        [BindProperty]
        public int Option { get; set; }

        public string Centro { get; set; }


        public async Task<IActionResult> OnGet()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;

            LstCentri = await _context.CentriLavs.Select(a => new SelectListItem
            {
                Value = a.IdCentroLavorazione.ToString(),
                Text = a.CentroLavDesc
            }).OrderBy(x=> x.Value).ToListAsync();

            Centro = CentroAppartenenza.GetCentroLavorazioneByUser(User);
            return Page();
        }



        public async Task<IActionResult> OnPostReportAsync()
        {
            try
            {
                int CentroID = CentroAppartenenza.SetCentroByRoleADMINSupervisor(User, SelectedCentro);
                Ruolo = User.FindFirstValue("Ruolo");


                if (ModelState.IsValid)
                {
                    await Search(StartDate, EndDate, CentroID, Option);
                    return Partial("_RiepilogoBancali", this);
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


        public async Task Search(DateTime startDate, DateTime endDate, int idCentro, int option)
        {
            string fromDate = startDate.Date.ToString("yyyyMMdd");
            string toDate = endDate.Date.ToString("yyyyMMdd");


            List<BancaleFuoriSlaView> LstBancaliFuoriSlaView = new List<BancaleFuoriSlaView>();
            string sql = "";
            string constr = _context.Database.GetDbConnection().ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {

                if (option ==1 )
                {
                    //tutti
                    if(idCentro == 5)
                    {
                        sql = @"SELECT t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale as DataArrivoBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc as CentroArrivo,t1.IdCentroArrivo
                                FROM [GIACENZA_SORTER_RM_TEST].[dbo].[Bancali] as t1
                                left join Commesse as t3 on t1.IdCommessa = t3.IdCommessa
                                left join CentriLav as t4 on t1.IdCentroArrivo = t4.IdCentroLavorazione
                                where convert(date,t1.dataAccettazioneBancale) >= {0} and convert(date,t1.dataAccettazioneBancale) <= {1} ";

                        using (SqlCommand cmd = new SqlCommand(sql))
                        {

                            cmd.Connection = con;
                            cmd.CommandTimeout = 60;
                            cmd.Parameters.Clear();
                                
                            string[] myParams = { fromDate, toDate };

                            LstBancaliFuoriSlaView = await _dbContext.Set<BancaleFuoriSlaView>().FromSqlRaw(sql, myParams).ToListAsync();
                        }
                    }
                    else
                    {
                        sql = @"SELECT t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale as DataArrivoBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc as CentroArrivo,t1.IdCentroArrivo
                                FROM [GIACENZA_SORTER_RM_TEST].[dbo].[Bancali] as t1
                                left join Commesse as t3 on t1.IdCommessa = t3.IdCommessa
                                left join CentriLav as t4 on t1.IdCentroArrivo = t4.IdCentroLavorazione
                                where convert(date,t1.dataAccettazioneBancale) >= {0} and convert(date,t1.dataAccettazioneBancale) <= {1} and t1.IdCentroArrivo = {2}";

                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = con;
                            cmd.CommandTimeout = 60;
                            cmd.Parameters.Clear();

                            string[] myParams = { fromDate, toDate, idCentro.ToString() };

                            LstBancaliFuoriSlaView = await _dbContext.Set<BancaleFuoriSlaView>().FromSqlRaw(sql, myParams).ToListAsync();
                        }
                    }
                }

                //non utilizzata perchè manca il fuori SLA
                //if (option == 2)
                //{
                //    //tutti
                //    if (idCentro == 5)
                //    {
                //        //sql = @"SELECT t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale as DataArrivoBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc as CentroArrivo,t1.IdCentroArrivo,
                //        //        DATEDIFF(day, t1.DataAccettazioneBancale, GETDAte()) AS FuoriSla
                //        //        FROM[GIACENZA_SORTER_RM_TEST].[dbo].[Bancali] as t1
                //        //        left join scatole as t2 on t1.IdBancale = t2.IdBancale
                //        //        left join Commesse as t3 on t1.IdCommessa = t3.IdCommessa
                //        //        left join CentriLav as t4 on t1.IdCentroArrivo = t4.IdCentroLavorazione
                //        //        where t2.IdBancale is null and DATEDIFF(day, t1.DataAccettazioneBancale, GETDAte()) > t3.GiorniSla ";


                //        sql = @"SELECT *
                //                FROM
                //                (
	               //                 SELECT t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale as DataArrivoBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc as CentroArrivo,t1.IdCentroArrivo,
	               //                 t1.dataSorter as DataSorter,t1.dataPrevistaFineSLa as DataPrevistaFineSLa,
	               //                 CASE
		              //                  WHEN t1.dataSorter is null THEN DATEDIFF(day,t1.dataPrevistaFineSLa, getdate()) 
		              //                  WHEN t1.dataSorter is not null THEN DATEDIFF(day, t1.dataPrevistaFineSLa, t1.dataSorter) 
	               //                 END AS FuoriSla
	               //                 FROM[GIACENZA_SORTER_RM_TEST].[dbo].[Bancali] as t1
	               //                 left join scatole as t2 on t1.IdBancale = t2.IdBancale
	               //                 left join Commesse as t3 on t1.IdCommessa = t3.IdCommessa
	               //                 left join CentriLav as t4 on t1.IdCentroArrivo = t4.IdCentroLavorazione
	               //                 group by t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc ,t1.IdCentroArrivo,
	               //                 t1.dataSorter,t1.dataPrevistaFineSLa
                //                ) as innerTable
                //                WHERE FuoriSla > 0 
                //                order by IdBancale";

                //        using (SqlCommand cmd = new SqlCommand(sql))
                //        {

                //            cmd.Connection = con;
                //            cmd.CommandTimeout = 60;

                //            LstBancaliFuoriSlaView = await _context.Set<BancaleFuoriSlaView>().FromSqlRaw(sql).ToListAsync();
                //        }

                //    }
                //    else
                //    {
                //        //sql = @"SELECT t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale as DataArrivoBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc as CentroArrivo,t1.IdCentroArrivo,
                //        //        DATEDIFF(day, t1.DataAccettazioneBancale, GETDAte()) AS FuoriSla
                //        //        FROM[GIACENZA_SORTER_RM_TEST].[dbo].[Bancali] as t1
                //        //        left join scatole as t2 on t1.IdBancale = t2.IdBancale
                //        //        left join Commesse as t3 on t1.IdCommessa = t3.IdCommessa
                //        //        left join CentriLav as t4 on t1.IdCentroArrivo = t4.IdCentroLavorazione
                //        //        where t2.IdBancale is null and DATEDIFF(day, t1.DataAccettazioneBancale, GETDAte()) > t3.GiorniSla and t1.IdCentroArrivo = {0}";

                //        sql = @"SELECT * 
                //                FROM
                //                (
	               //                 SELECT t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale as DataArrivoBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc as CentroArrivo,t1.IdCentroArrivo,
	               //                 t1.dataSorter as DataSorter,t1.dataPrevistaFineSLa as DataPrevistaFineSLa,
	               //                 CASE
		              //                  WHEN t1.dataSorter is null THEN DATEDIFF(day,t1.dataPrevistaFineSLa, getdate()) 
		              //                  WHEN t1.dataSorter is not null THEN DATEDIFF(day, t1.dataPrevistaFineSLa, t1.dataSorter) 
	               //                 END AS FuoriSla
	               //                 FROM[GIACENZA_SORTER_RM_TEST].[dbo].[Bancali] as t1
	               //                 left join scatole as t2 on t1.IdBancale = t2.IdBancale
	               //                 left join Commesse as t3 on t1.IdCommessa = t3.IdCommessa
	               //                 left join CentriLav as t4 on t1.IdCentroArrivo = t4.IdCentroLavorazione
	               //                 group by t1.IdBancale,t1.bancale,t1.DataAccettazioneBancale,t1.OperatoreAccettazione,t1.Note, t3.Commessa,t3.GiorniSla,t4.CentroLavDesc ,t1.IdCentroArrivo,
	               //                 t1.dataSorter,t1.dataPrevistaFineSLa
                //                ) as innerTable
                //                WHERE FuoriSla > 0 and t1.IdCentroArrivo = {0}
                //                order by IdBancale";

                //        using (SqlCommand cmd = new SqlCommand(sql))
                //        {

                //            cmd.Connection = con;
                //            cmd.CommandTimeout = 60;
                //            cmd.Parameters.Clear();

                //            string[] myParams = {idCentro.ToString() };

                //            LstBancaliFuoriSlaView = await _context.Set<BancaleFuoriSlaView>().FromSqlRaw(sql, myParams).ToListAsync();
                //        }
                //    }
                //}
            }


            LstBancaleView = LstBancaliFuoriSlaView.Select(m => new BancaleView
            {
                IdBancale = m.IdBancale,
                Bancale = m.Bancale,
                DataArrivoBancale = m.DataArrivoBancale,
                OperatoreAccettazione = m.OperatoreAccettazione,
                Note = m.Note,
                Commessa = m.Commessa,
                Elimina = false,
                CentroArrivo = m.CentroArrivo

            }).OrderBy(x => x.IdBancale).ToList();

            if (LstBancaleView.Count == 0)
            {
                Message = "Not results found";
            }



        }


        //Cascading Delete within SQL Server
        public async Task<IActionResult> OnPostElimina()
        {
            var lst = LstBancaleView.Where(x => x.Elimina == true).ToList();
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    Bancali bancale = await _context.Bancalis.FindAsync(item.IdBancale);
                    if (bancale != null)
                    {
                        try
                        {
                            _context.Bancalis.Remove(bancale);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Delete bancale : " + bancale.Bancale + " - operatore : " + User.Identity.Name);
                        }
                        catch (Exception ex)
                        {
                            Message = "Attenzione il bancale " + bancale.Bancale + " contiene scatole normalizzate";
                            _logger.LogError("Delete bancale : " + bancale.Bancale + ex.Message);
                        }

                        await Search(StartDate, EndDate, bancale.IdCentroArrivo, Option);
                        return Partial("_RiepilogoBancali", this);
                    }
                }
            }

            
            return new EmptyResult();
        }





    }
}
