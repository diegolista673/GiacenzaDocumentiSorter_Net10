using System;
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class BancaleView
    {
        public int IdBancale { get; set; }
        public string Bancale { get; set; } = string.Empty;

        [Display(Name = "Data Arrivo")]
        [DataType(DataType.Date)]
        public DateTime? DataArrivoBancale { get; set; }

        [Display(Name = "Operatore Accettazione")]
        public string OperatoreAccettazione { get; set; } = string.Empty;

        [Display(Name = "Data Invio")]
        [DataType(DataType.Date)]
        public DateTime? DataInvioBancale { get; set; }

        [Display(Name = "Operatore Invio")]
        public string? OperatoreInvio { get; set; }
        
        public string? Note { get; set; }

        public string Commessa { get; set; } = string.Empty;

        public string CentroArrivo { get; set; } = string.Empty;

        public int TotaleBancale { get; set; }

        public bool Elimina { get; set; }

        [Display(Name = "Giorni fuori SLA")]
        public int FuoriSLa { get; set; }

        [Display(Name = "Data Sorter")]
        [DataType(DataType.Date)]
        public DateTime? DataSorter { get; set; }

        [Display(Name = "Fine SLA")]
        [DataType(DataType.Date)]
        public DateTime? DataPrevistaFineSLa { get; set; }
    }
}
