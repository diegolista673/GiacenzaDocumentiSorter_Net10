using System;
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models

{
    public class ScatolaDto
    {
        [Required(ErrorMessage = "Data is required")]
        public DateTime? DataCambioGiacenza { get; set; }

        [Required(ErrorMessage = "Tipo IdCentroCambioGiacenza is required")]
        public int? IdCentroGiacenza { get; set; }

        public string? NoteCambioGiacenza { get; set; }

        [Required(ErrorMessage = "Scatola is required")]
        public string Scatola { get; set; } = string.Empty;

        public string OperatoreCambioGiacenza { get; set; } = string.Empty;
    }
}
