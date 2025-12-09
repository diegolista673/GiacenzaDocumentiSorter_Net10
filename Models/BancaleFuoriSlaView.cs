using System;
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class BancaleFuoriSlaView
    {
        public int IdBancale { get; set; }
        
        public string Bancale { get; set; } = string.Empty;

        [Display(Name = "Data Arrivo")]
        [DataType(DataType.Date)]
        public DateTime? DataArrivoBancale { get; set; }

        [Display(Name = "Operatore Accettazione")]
        public string OperatoreAccettazione { get; set; } = string.Empty;

        public string? Note { get; set; }

        public string Commessa { get; set; } = string.Empty;

        public string CentroArrivo { get; set; } = string.Empty;

        public int IdCentroArrivo { get; set; }
    }
}
