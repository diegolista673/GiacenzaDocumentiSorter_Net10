using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsContenitori))]
    public partial class Contenitori
    {
        // leave it empty.
    }

    public class NewsAttribsContenitori
    {

        //Contenitori
        [StringLength(100)]
        public string Contenitore { get; set; }


        [Display(Name = "Totale Documenti")]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be equal or greater than {1}.")]
        public int? TotaleDocumenti { get; set; }// Your attribs will come here.
    }
}
