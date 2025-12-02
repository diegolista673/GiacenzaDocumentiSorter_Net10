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
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Pages.PagesSorter
{
    [Authorize(Policy = "SorterRequirements")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;

        public List<Scatole> ScatoleLst { get; set; }

        public ScatoleModel ScatoleModel { get; set; }

        public string Message { get; set; }

        [BindProperty]
        public Scatole Scatole { get; set; }


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

                var ScatolaGiacenza = await _context.Scatoles.Where(x => x.Scatola == Scatole.Scatola).FirstOrDefaultAsync();

                if (ScatolaGiacenza != null)
                {
                    //se scatola è già stata sorterizzata
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

                        var scatola = _context.Scatoles.SingleOrDefault(b => b.Scatola == Scatole.Scatola);
                        scatola.DataSorter = Scatole.DataSorter;
                        scatola.IdCentroSorterizzazione = CentroAppartenenza.SetCentroByUser(User);
                        scatola.IdStato = 2;
                        scatola.OperatoreSorter = User.Identity.Name;
                        scatola.Note = Scatole.Note;

                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Inserimento scatola sorterizzata : " + Scatole.Scatola + " - operatore : " + User.Identity.Name);

                        ScatoleModel = new ScatoleModel
                        {
                            LastScatola = Scatole.Scatola,
                            ScatoleLst = await GetListScatole(Scatole.DataSorter),
                            Default = true
                        };
                    }
                }
                else
                {
                    //se scatola non esiste chiede di normalizzarla prima
                    ScatoleModel = new ScatoleModel
                    {
                        LastScatola = Scatole.Scatola,
                        ScatoleLst = await GetListScatole(Scatole.DataSorter),
                        IsNormalizzata = false,
                        IsSorterizzata = false,
                        Default = false
                    };
                }












                ////se scatola non esiste chiede di normalizzarla prima
                //if (!ScatolaExists(Scatole.Scatola))
                //{
                //    ScatoleModel = new ScatoleModel
                //    {
                //        LastScatola = Scatole.Scatola,
                //        ScatoleLst = await GetListScatole(Scatole.DataSorter),
                //        IsNormalizzata = false,
                //        IsSorterizzata = false,
                //        Default = false
                //    };

                //}
                //else
                //{
                //    //se scatola è già stata sorterizzata
                //    if (ScatolaSorterizzataExist(Scatole.Scatola)) {

                //        ScatoleModel = new ScatoleModel
                //        {
                //            LastScatola = Scatole.Scatola,
                //            ScatoleLst = await GetListScatole(Scatole.DataSorter),
                //            IsNormalizzata = true,
                //            IsSorterizzata = true,
                //            Default = false
                //        };

                //    }
                //    else
                //    {

                //        var scatola = _context.Scatoles.SingleOrDefault(b => b.Scatola == Scatole.Scatola);
                //        scatola.DataSorter = Scatole.DataSorter;
                //        scatola.IdCentroSorterizzazione = CentroAppartenenza.SetCentroByUser(User);
                //        scatola.IdStato = 2;
                //        scatola.OperatoreSorter = User.Identity.Name;
                //        scatola.Note = Scatole.Note;

                //        await _context.SaveChangesAsync();

                //        _logger.LogInformation("Inserimento scatola sorterizzata : " + Scatole.Scatola + " - operatore : " + User.Identity.Name);

                //        ScatoleModel = new ScatoleModel
                //        {
                //            LastScatola = Scatole.Scatola,
                //            ScatoleLst = await GetListScatole(Scatole.DataSorter),
                //            Default = true
                //        };
                //    }


                //}

                return Partial("_RiepilogoScatoleSorter", ScatoleModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Partial("_RiepilogoScatoleSorter", ScatoleModel);
            }
 
        }


        //private bool ScatolaExists(string scatola)
        //{
        //    return _context.Scatoles.Any(e => e.Scatola == scatola);
        //}

        //private bool ScatolaSorterizzataExist(string scatola)
        //{
        //    return _context.Scatoles.Any(e => e.Scatola == scatola && e.DataSorter != null);
        //}

        private bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }

        public void InitializePage()
        {

            TimeSpan start = new TimeSpan(24, 0, 0);
            TimeSpan end = new TimeSpan(6, 0, 0);
            bool res = TimeBetween(DateTime.Now, start, end);

            DateTime dataSorter;
            if (res)
            {
                dataSorter = DateTime.Now.AddDays(-1); 
            }
            else
            {
                dataSorter = DateTime.Now;
            }
                
            
            Scatole = new Scatole
            {
                DataSorter = dataSorter


            };

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
                                           .Include(s => s.IdTipologiaNavigation).Where(x => x.DataSorter == dataLavorazione.Value.Date).OrderBy(x => x.IdScatola).ToListAsync();

            }
            else
            {
                int idCentro = CentroAppartenenza.SetCentroByUser(User);

                ScatoleLst = await _context.Scatoles
                                           .Include(s => s.IdCommessaNavigation)
                                           .Include(s => s.IdContenitoreNavigation)
                                           .Include(s => s.IdStatoNavigation)
                                           .Include(s => s.IdTipoNormalizzazioneNavigation)
                                           .Include(s => s.IdTipologiaNavigation).Where(x => x.DataSorter == dataLavorazione.Value.Date && x.IdCentroSorterizzazione == idCentro).OrderBy(x => x.IdScatola).ToListAsync();
            }

            return ScatoleLst;

        }
    }
}
