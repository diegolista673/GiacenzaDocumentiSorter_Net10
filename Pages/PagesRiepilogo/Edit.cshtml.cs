using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using Microsoft.AspNetCore.Authorization;

namespace GiacenzaSorterRm.Pages.PagesRiepilogo
{
    [Authorize(Policy = "SorterRequirements")]
    public class EditModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<EditModel> _logger;


        public EditModel(ILogger<EditModel> logger,  GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        public SelectList CommesseSL { get; set; }
        public SelectList TipologieSL { get; set; }
        public SelectList ContenitoriSL { get; set; }
        public SelectList TipoNormSL { get; set; }


        [BindProperty]
        public Scatole Scatole { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Scatole = await _context.Scatoles
                                    .Include(s => s.IdCommessaNavigation)
                                    .Include(s => s.IdContenitoreNavigation)
                                    .Include(s => s.IdTipoNormalizzazioneNavigation)
                                    .Include(s => s.IdTipologiaNavigation).FirstOrDefaultAsync(m => m.IdScatola == id);

            if (Scatole == null)
            {
                return NotFound();
            }


            CommesseSL = new SelectList(_context.Commesses, "IdCommessa", "Commessa");
            TipologieSL = new SelectList(_context.Tipologies, "IdTipologia", "Tipologia");
            ContenitoriSL = new SelectList(_context.Contenitoris, "IdContenitore", "Contenitore");
            TipoNormSL = new SelectList(_context.TipiNormalizzaziones, "IdTipoNormalizzazione", "TipoNormalizzazione");

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {

                CommesseSL = new SelectList(_context.Commesses, "IdCommessa", "Commessa");
                TipologieSL = new SelectList(_context.Tipologies, "IdTipologia", "Tipologia");
                ContenitoriSL = new SelectList(_context.Contenitoris, "IdContenitore", "Contenitore");
                TipoNormSL = new SelectList(_context.TipiNormalizzaziones, "IdTipoNormalizzazione", "TipoNormalizzazione");
                return Page();
            }

            

            if(Scatole.DataSorter != null)
            {
                Scatole.IdStato = 2;
                Scatole.OperatoreSorter = User.Identity.Name;
            }
            else
            {
                Scatole.IdStato = 1;
                Scatole.OperatoreNormalizzazione = User.Identity.Name;
            }
            
            _context.Attach(Scatole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Modified scatola normalizzata : " + Scatole.Scatola + " - operatore : " + User.Identity.Name);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScatoleExists(Scatole.IdScatola))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ScatoleExists(int id)
        {
            return _context.Scatoles.Any(e => e.IdScatola == id);
        }
    }
}
