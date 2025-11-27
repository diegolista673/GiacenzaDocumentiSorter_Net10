using System.ComponentModel.DataAnnotations;


namespace GiacenzaSorterRm.Models
{
    public class MaceroView
    {
        public string? Centro { get; set; }

        public string? Piattaforma { get; set; }
        
        public string? Commessa { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int? Giacenza_Documenti { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int? Numero_Scatole { get; set; }

    }
}
