using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GiacenzaSorterRm.Models.Database;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace GiacenzaSorterRm.Pages.TipiDocumenti
{
    [Authorize(Roles = "ADMIN, SUPERVISOR")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterContext _context;
        private readonly ILogger<CreateModel> _logger;


        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
        {
            _logger = logger;
            _context = context;
        }



        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Tipologie Tipologie { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            try
            {
                Tipologie.DataCreazione = DateTime.Now.Date;
                Tipologie.IdOperatoreCreazione = Int32.Parse(User.FindFirst("IdOperatore").Value);
                _context.Tipologies.Add(Tipologie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The name is already in use.");
                return Page();
            }

            _logger.LogInformation("Tipologia documento creata: {Tipologia}", Tipologie.Tipologia);
            return RedirectToPage("./Index");
        }
    }
}
