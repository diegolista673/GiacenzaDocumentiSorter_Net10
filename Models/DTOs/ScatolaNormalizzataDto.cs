using GiacenzaSorterRm.AppCode;
using System;
using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models.DTOs
{
    /// <summary>
    /// DTO per visualizzazione scatole normalizzate nel riepilogo.
    /// Contiene solo i campi necessari per la view _RiepilogoNormalizzateInserite
    /// </summary>
    public class ScatolaNormalizzataDto
    {
        public int IdScatola { get; set; }

        [Required(ErrorMessage = "Scatola is required")]
        [StringLength(100)]
        public string Scatola { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Data Normalizzazione")]
        [Required(ErrorMessage = "inserire una data valida")]
        [DateLessEqualThan("DataSorter", ErrorMessage = "Data maggiore di data Sorter")]
        public DateTime DataNormalizzazione { get; set; }

        [Display(Name = "Operatore Normalizzazione")]
        public string OperatoreNormalizzazione { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contenitore is required")]
        public int IdContenitore { get; set; } 

        [Required(ErrorMessage = "Commessa is required")]
        public int IdCommessa { get; set; } 

        [Required(ErrorMessage = "Tipologia is required")]
        public int IdTipologia { get; set; } 

        [Required(ErrorMessage = "Tipo normalizzazione is required")]
        public int IdTipoNormalizzazione { get; set; } 


        public int IdCentroNormalizzazione { get; set; } 
        public string Commessa { get; set; } = string.Empty;
        public string Contenitore { get; set; } = string.Empty;
        public string Tipologia { get; set; } = string.Empty;
        public string TipoNormalizzazione { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int IdStato { get; set; } 

    }
}
