using GiacenzaSorterRm.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class BancaliModel
    {
        public BancaliModel() { }

        public List<Bancali> BancaliLst { get; set; }
        public string LastBancale { get; set; }

        public bool IsAccettata { get; set; }

        public bool IsNotConforme { get; set; }

        public bool Default { get; set; }

        public string PdfBancale { get; set; }

        string _alertAccettata;
        public string AlertAccettata
        {
            get
            {

                if (IsAccettata)
                {
                    this._alertAccettata = "Il bancale risulta inserito";
                }


                return this._alertAccettata;
            }

        }



        string _alertNonConforme;
        public string AlertNonConforme
        {
            get
            {

                if (IsNotConforme)
                {
                    this._alertNonConforme = "Nome bancale non conforme, bancale non inserito in DB";
                }

                return this._alertNonConforme;
            }

        }


    }
}
