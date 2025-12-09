using System;
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class ScatolaView
    {
        public int IdScatola { get; set; }
        public string Scatola { get; set; } = string.Empty;

        [Display(Name = "Normalizzazione")]
        [DataType(DataType.Date)]
        public DateTime? DataNormalizzazione { get; set; }

        [Display(Name = "Operatore Normalizzazione")]
        public string? OperatoreNormalizzazione { get; set; }

        [Display(Name = "Sorter")]
        [DataType(DataType.Date)]
        public DateTime? DataSorter { get; set; }

        [Display(Name = "Operatore Sorter")]
        public string? OperatoreSorter { get; set; }
        
        public string? Note { get; set; }
        
        public string Stato { get; set; } = string.Empty;
        
        public string Contenitore { get; set; } = string.Empty;
        
        public string Commessa { get; set; } = string.Empty;
        
        public string Tipologia { get; set; } = string.Empty;
        
        public int TotaleScatola { get; set; }
        
        public string TipoProdotto { get; set; } = string.Empty;
        
        public bool Elimina { get; set; }

        [Display(Name = "Accettazione")]
        [DataType(DataType.Date)]
        public DateTime? DataArrivoBancale { get; set; }
        
        public int IdBancale { get; set; }
        
        public int IdCentroNormalizzazione { get; set; }
        
        public int? IdCentroSorterizzazione { get; set; }

        public string CentroNormalizzazione { get; set; } = string.Empty;
        
        public string? CentroSorterizzazione { get; set; }
    }
}
