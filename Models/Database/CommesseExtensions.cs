using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsCommesse))]
    public partial class Commesse
    {
        // leave it empty.
    }

    public class NewsAttribsCommesse
    {
        //Commesse
        [Required(AllowEmptyStrings = false)]
        [StringLength(200)]
        public required string Commessa { get; set; }

        [Display(Name = "Data Creazione")]
        [DataType(DataType.Date)]
        public DateTime? DataCreazione { get; set; }


    }
}
