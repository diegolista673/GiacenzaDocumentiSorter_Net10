using GiacenzaSorterRm.Models.Database;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models
{
    public class ScatoleModel
    {
        public ScatoleModel() { }

        public List<Scatole>? ScatoleLst { get; set; }
        
        public string LastScatola { get; set; } = string.Empty;

        public bool IsNormalizzata { get; set; }
        
        public bool IsSorterizzata { get; set; }
        
        public bool IsNotConforme { get; set; }

        public bool Default { get; set; }

        private string? _alertNormalizzata;
        
        public string? AlertNormalizzata
        {
            get
            {
                if (IsNormalizzata)
                {
                    _alertNormalizzata = "La scatola risulta inserita";
                }

                return _alertNormalizzata;
            }
        }

        private string? _alertSorterizzata;
        
        public string? AlertSorterizzata
        {
            get
            {
                if (Default)
                {
                    _alertSorterizzata = "";
                }
                else if (IsSorterizzata)
                {
                    _alertSorterizzata = "La scatola risulta inserita";
                }
                else if (IsSorterizzata == false && IsNormalizzata == false ) 
                { 
                    _alertSorterizzata = "Aggiornamento non riuscito. La scatola non è stata normalizzata in precedenza";
                }

                return _alertSorterizzata;
            }
        }

        private string? _alertNonConforme;
        
        public string? AlertNonConforme
        {
            get
            {
                if (IsNotConforme)
                {
                    _alertNonConforme = "Nome scatola non conforme, scatola non inserita in DB";
                }

                return _alertNonConforme;
            }
        }

        public bool ScatolaNonPresenteMondo { get; set; }

        public bool ScatolaSorterizzata { get; set; }

        public bool ScatolaNonPresenteInDB { get; set; }
    }
}
