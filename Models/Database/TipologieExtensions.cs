using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsTipologie))]
    public partial class Tipologie
    {
        // leave it empty.
    }

    public class NewsAttribsTipologie
    {
        //Tipologie
        [StringLength(100)]
        public string Tipologia { get; set; }
    }
}
