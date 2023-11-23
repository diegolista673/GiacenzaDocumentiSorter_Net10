using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class BancaleFuoriSlaView
    {
        public int IdBancale { get; set; }
        public string Bancale { get; set; }

        [Display(Name = "Data Arrivo")]
        [DataType(DataType.Date)]
        public DateTime? DataArrivoBancale { get; set; }

        [Display(Name = "Operatore Accettazione")]
        public string OperatoreAccettazione { get; set; }

        public string Note { get; set; }


        public string Commessa { get; set; }

        public string CentroArrivo { get; set; }

        public int IdCentroArrivo { get; set; }






    }
}
