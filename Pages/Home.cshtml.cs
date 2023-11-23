using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GiacenzaSorterRm.AppCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GiacenzaSorterRm.Pages
{
 
    public class HomeModel : PageModel
    {
        private readonly ILogger<HomeModel> _logger;

        public HomeModel(ILogger<HomeModel> logger)
        {

            _logger = logger;


        }

        public void OnGet()
        {
            _logger.LogInformation("Accesso eseguito da :" + User.Identity.Name);
        }

    }
}