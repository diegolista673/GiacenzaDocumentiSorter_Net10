using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsTipiNormalizzazione))]
    public partial class TipiNormalizzazione
    {
        // leave it empty.
    }

    public class NewsAttribsTipiNormalizzazione
    {

        //TipiNormalizzazione
        [StringLength(100)]
        [Display(Name = "Tipo Prodotto")]
        public string TipoNormalizzazione { get; set; }
    }
}
