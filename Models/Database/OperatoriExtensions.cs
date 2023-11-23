using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsOperatori))]
    public partial class Operatori
    {
        // leave it empty.
    }

    public class NewsAttribsOperatori
    {

        [Display(Name = "Centro")]
        public string CentroLavDesc { get; set; }


        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Centro is required")]
        [Required]
        public int? IdCentroLav { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Ruolo is required")]
        public string Ruolo { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Azienda is required")]
        public string Azienda { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(8, ErrorMessage = "Must be 8 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Operatore is required")]
        public string Operatore { get; set; }

    }
}
