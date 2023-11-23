using GiacenzaSorterRm.AppCode;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsScatole))]
    public partial class Scatole
    {
        // leave it empty.
    }

    public class NewsAttribsScatole
    {
        //Scatole
        [Required(ErrorMessage = "Scatola is required")]
        [StringLength(100)]
        public string Scatola { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Normalizzazione")]
        [Required(ErrorMessage = "inserire una data valida")]
        [DateLessEqualThan("DataSorter", ErrorMessage = "Data maggiore di data Sorter")]
        public DateTime DataNormalizzazione { get; set; }


        [Display(Name = "Operatore Normalizzazione")]
        public string OperatoreNormalizzazione { get; set; }


        [DateGreaterThan("DataNormalizzazione", ErrorMessage = "Data minore di data Normalizzazione")]
        [Display(Name = "Data Sorter")]
        [DataType(DataType.Date)]
        public DateTime? DataSorter { get; set; }


        [Display(Name = "Operatore Sorter")]
        public string OperatoreSorter { get; set; }


        [Required(ErrorMessage = "Contenitore is required")]
        public int IdContenitore { get; set; }


        [Required(ErrorMessage = "Commessa is required")]
        public int IdCommessa { get; set; }

        
        [Required(ErrorMessage = "Tipologia is required")]
        public int IdTipologia { get; set; }

        
        [Required(ErrorMessage = "Tipo normalizzazione is required")]
        public int IdTipoNormalizzazione { get; set; }


        [DateGreaterThan("DataNormalizzazione", ErrorMessage = "Data minore di data Normalizzazione")]
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime? DataCambioGiacenza { get; set; }



    }


}
