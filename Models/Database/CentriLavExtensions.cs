using GiacenzaSorterRm.AppCode;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsCentriLav))]
    public partial class CentriLav
    {
        // leave it empty.
    }

    public class NewsAttribsCentriLav
    {

        [Display(Name = "Centro")]
        public string CentroLavDesc { get; set; }

    }


}
