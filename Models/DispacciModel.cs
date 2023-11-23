using GiacenzaSorterRm.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class DispacciModel
    {
        public DispacciModel() { }

        public List<string> ElencoDispacci { get; set; } = new List<string>();
        public string Dispaccio { get; set; }

    }
}
