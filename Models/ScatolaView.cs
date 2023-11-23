using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class ScatolaView
    {
        public int IdScatola { get; set; }
        public string Scatola { get; set; }

        [Display(Name = "Normalizzazione")]
        [DataType(DataType.Date)]
        public DateTime? DataNormalizzazione { get; set; }

        [Display(Name = "Operatore Normalizzazione")]
        public string OperatoreNormalizzazione { get; set; }

        [Display(Name = "Sorter")]
        [DataType(DataType.Date)]
        public DateTime? DataSorter { get; set; }

        [Display(Name = "Operatore Sorter")]
        public string OperatoreSorter { get; set; }
        public string Note { get; set; }
        public string Stato { get; set; }
        public string Contenitore { get; set; }
        public string Commessa { get; set; }
        public string Tipologia { get; set; }
        public int TotaleScatola { get; set; }
        public string TipoProdotto { get; set; }
        public bool Elimina { get; set; }

        [Display(Name = "Accettazione")]
        [DataType(DataType.Date)]
        public DateTime? DataArrivoBancale { get; set; }
        public int IdBancale { get; set; }
        public int IdCentroNormalizzazione { get; set; }
        public int? IdCentroSorterizzazione { get; set; }

       
        public string CentroNormalizzazione { get; set; }
        public string CentroSorterizzazione { get; set; }
    }
}
