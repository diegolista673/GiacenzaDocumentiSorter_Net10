using System;
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


namespace GiacenzaSorterRm.Pages.PagesAssociazione
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;

        public SelectList CommesseSL { get; set; }
        public SelectList TipologieSL { get; set; }
        public SelectList ContenitoriSL { get; set; }


        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult OnGet()
        {
            CommesseSL = new SelectList(_context.Commesses.OrderBy(x => x.Commessa), "IdCommessa", "Commessa");
            TipologieSL = new SelectList(_context.Tipologies.OrderBy(x=> x.Tipologia), "IdTipologia", "Tipologia");
            ContenitoriSL = new SelectList(_context.Contenitoris.OrderBy(x => x.Contenitore), "IdContenitore", "Contenitore");

            return Page();
        }

        [BindProperty]
        public CommessaTipologiaContenitore Ctc { get; set; }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Ctc.DescCommessa = await _context.Commesses.Where(p => p.IdCommessa == Ctc.IdCommessa).Select(x=> x.Commessa).FirstAsync();
                Ctc.DescTipologia = await _context.Tipologies.Where(p => p.IdTipologia == Ctc.IdTipologia).Select(x => x.Tipologia).FirstAsync();
                Ctc.DescContenitore = await _context.Contenitoris.Where(p => p.IdContenitore == Ctc.IdContenitore).Select(x => x.Contenitore).FirstAsync();
                Ctc.Quantita = await _context.Contenitoris.Where(p => p.IdContenitore == Ctc.IdContenitore).Select(x => x.TotaleDocumenti).FirstAsync();

                _context.CommessaTipologiaContenitores.Add(Ctc);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. " + "The record is already in use.");
                _logger.LogError(ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError(ex.Message);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
