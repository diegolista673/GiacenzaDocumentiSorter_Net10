using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GiacenzaSorterRm.Data;
using System.Security.Claims;

namespace GiacenzaSorterRm.Pages.TipiLavorazioni
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger, IAppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<SelectListItem> LstCentri { get; set; }
        public string Ruolo { get; set; }
        public string Utente { get; set; }


        [BindProperty]
        public int SelectedCentro { get; set; }


        [BindProperty]
        public Commesse Commesse { get; set; }

        public SelectList PiattaformeSL { get; set; }


        public IActionResult OnGet()
        {
            PiattaformeSL = new SelectList(_context.Piattaformes, "IdPiattaforma", "Piattaforma");
            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;


            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                
                Commesse.DataCreazione = DateTime.Now.Date;
                Commesse.IdOperatore = Int32.Parse(User.FindFirst("IdOperatore").Value);
                Commesse.GiorniSla = 1;
                Commesse.Attiva = true;

                _context.Commesses.Add(Commesse);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The name is already in use.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
