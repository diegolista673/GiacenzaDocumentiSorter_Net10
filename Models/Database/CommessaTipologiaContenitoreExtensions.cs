using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsCommessaTipologiaContenitore))]
    public partial class CommessaTipologiaContenitore
    {
        // leave it empty.
    }

    public class NewsAttribsCommessaTipologiaContenitore
    {
        //CommessaTipologiaContenitore
        [Display(Name = "Commessa")]
        public string DescCommessa { get; set; }

        [Display(Name = "Tipologia")]
        public string DescTipologia { get; set; }

        [Display(Name = "Contenitore")]
        public string DescContenitore { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public string Quantita { get; set; }
    }
}
