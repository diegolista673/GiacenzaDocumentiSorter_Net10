using GiacenzaSorterRm.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class CommesseView
    {
                 
        public string Commessa { get; set; }
        public string Tipologia { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        [Display(Name = "Documenti Normalizzati")]
        public int TotaleDocumentiNormalizzati { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        [Display(Name = "Documenti Sorter")]
        public int TotaleDocumentiSorterizzati { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        [Display(Name = "Documenti Giacenza")]
        public int TotaleDocumentiGiacenza { get; set; }
                
        [Display(Name = "Scatola più vecchia ancora da sorterizzare")]
        public string ScatolaNormalizzataOld { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Scatola più vecchia")]
        public DateTime? DataScatolaOld { get; set; }

        public string CentroNormalizzazione { get; set; }

        public string CentroGiacenza { get; set; }

        public string Bancale { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Bancale più vecchia")]
        public DateTime? DataBancaleOld { get; set; }
    }
}
