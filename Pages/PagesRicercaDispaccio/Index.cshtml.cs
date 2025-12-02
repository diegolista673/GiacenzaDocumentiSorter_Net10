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
using Microsoft.Data.SqlClient;
using GiacenzaSorterRm.Models.Database;

namespace GiacenzaSorterRm.Pages.PagesRicercaDispaccio
{
    public class IndexModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(ILogger<IndexModel> logger, GiacenzaSorterContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [BindProperty]
        [Required]
        public string ItemRicerca { get; set; }

        public string Ruolo { get; set; }
        public string Utente { get; set; }
        public string Centro { get; set; }
        public string Message { get; set; }

        public List<SelectListItem> LstRicerca { get; set; }

        [BindProperty]
        [Required]
        public string SelectedItem { get; set; }


        [BindProperty]
        public List<BancaliDispacci> LstBancaliDispacci { get; set; }

        public async Task<IActionResult> OnGet()
        {
            this.Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;
            Centro = CentroAppartenenza.GetCentroLavorazioneByUser(User);

            var lst = new List<string>() { "BANCALE", "DISPACCIO" };
            LstRicerca = lst.Select(x => new SelectListItem { Text = x, Value = x }).ToList();

            return Page();
        }


        public async Task<IActionResult> OnPostReportAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (SelectedItem == "BANCALE")
                    {
                        LstBancaliDispacci = await _context.BancaliDispaccis.Where(x => x.Bancale == ItemRicerca).ToListAsync();
                    }
                    else
                    {
                        LstBancaliDispacci = await _context.BancaliDispaccis.Where(x => x.Dispaccio == ItemRicerca).ToListAsync();
                    }


                    if (LstBancaliDispacci.Count() == 0)
                    {
                        Message = "Not results found";
                    }

                    return Partial("_RiepilogoDispacci", this);
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


    }
}
