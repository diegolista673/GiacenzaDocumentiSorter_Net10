using GiacenzaSorterRm.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Models
{
    public class ScatoleModel
    {
        public ScatoleModel() { }

        public List<Scatole> ScatoleLst { get; set; }
        public string LastScatola { get; set; }

        public bool IsNormalizzata { get; set; }
        public bool IsSorterizzata { get; set; }
        public bool IsNotConforme { get; set; }

        public bool Default { get; set; }


        string _alertNormalizzata;
        public string AlertNormalizzata
        {
            get
            {

                if (IsNormalizzata)
                {
                    this._alertNormalizzata = "La scatola risulta inserita";
                }


                return this._alertNormalizzata;
            }

        }


        string _alertSorterizzata;
        public string AlertSorterizzata
        {
            get
            {
                if (Default)
                {
                    this._alertSorterizzata = "";
                }
                else if (IsSorterizzata)
                {
                    this._alertSorterizzata = "La scatola risulta inserita";
                }
                else if (IsSorterizzata == false && IsNormalizzata == false ) 
                { 
                    this._alertSorterizzata = "Aggiornamento non riuscito. La scatola non è stata normalizzata in precedenza";
                }

                return this._alertSorterizzata;
            }

        }


        string _alertNonConforme;
        public string AlertNonConforme
        {
            get
            {

                if (IsNotConforme)
                {
                    this._alertNonConforme = "Nome scatola non conforme, scatola non inserita in DB";
                }

                return this._alertNonConforme;
            }

        }

        public bool ScatolaNonPresenteMondo { get; set; }

        public bool ScatolaSorterizzata { get; set; }

        public bool ScatolaNonPresenteInDB { get; set; }

    }
}
