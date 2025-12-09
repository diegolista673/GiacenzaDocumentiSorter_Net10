using System.Collections.Generic;

namespace GiacenzaSorterRm.Models
{
    public class DispacciModel
    {
        public DispacciModel() { }

        public List<string> ElencoDispacci { get; set; } = new List<string>();
        
        public string Dispaccio { get; set; } = string.Empty;
    }
}
