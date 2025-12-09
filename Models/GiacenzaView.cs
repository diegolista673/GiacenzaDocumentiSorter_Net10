using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class GiacenzaView
    {
        public string? Centro { get; set; }

        public string? Piattaforma { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int Giacenza { get; set; }
    }
}
