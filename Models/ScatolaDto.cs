using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class ScatolaDto
    {
        [Required(ErrorMessage = "Data is required")]
        public DateTime? DataCambioGiacenza { get; set; }


        [Required(ErrorMessage = "Tipo IdCentroCambioGiacenza is required")]
        public int? IdCentroGiacenza { get; set; }

        public string NoteCambioGiacenza { get; set; }


        [Required(ErrorMessage = "Scatola is required")]
        public string Scatola { get; set; }


        public string OperatoreCambioGiacenza { get; set; }


    }
}
