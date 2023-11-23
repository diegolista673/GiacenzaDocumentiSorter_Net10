using GiacenzaSorterRm.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class MaceroView
    {
        public string? Centro { get; set; }

        public string? Piattaforma { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int? Giacenza_Documenti { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int? Numero_Scatole { get; set; }

    }
}
