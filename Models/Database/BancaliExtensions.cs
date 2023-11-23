using GiacenzaSorterRm.AppCode;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models.Database
{
    [ModelMetadataType(typeof(NewsAttribsBancali))]
    public partial class Bancali
    {
        // leave it empty.
    }

    public class NewsAttribsBancali
    {
        //Scatole
        //[Required(ErrorMessage = "Bancale is required")]
        [StringLength(100)]
        public string Bancale { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Accettazione")]
        [Required(ErrorMessage = "inserire una data valida")]
        public DateTime DataAccettazioneBancale { get; set; }

        [Display(Name = "Operatore Accettazione")]
        public string OperatoreAccettazione { get; set; }


        [Display(Name = "Data Invio Altro Centro")]
        [DataType(DataType.Date)]
        public DateTime? DataInvioAltroCentro { get; set; }

        [Display(Name = "Operatore Invio")]
        public string OperatoreInvio { get; set; }


        [Required(ErrorMessage = "Commessa is required")]
        public int IdCommessa { get; set; }


    }


}
