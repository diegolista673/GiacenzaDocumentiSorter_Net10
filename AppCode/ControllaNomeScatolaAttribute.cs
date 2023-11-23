using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace GiacenzaSorterRm.Models
{

    public class ControllaNomeScatolaAttribute : ValidationAttribute
    {
        public ControllaNomeScatolaAttribute()
        {
            
        }
        //AMA RM BUSTE00012364AMA RM BUSTE00012364
        //public string GetErrorMessage() =>  $"Controllare la scatola inserita";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var sc = (Scatole)validationContext.ObjectInstance;

            if (!string.IsNullOrEmpty(sc.Scatola) && (sc.Scatola.Length >=8))
            {
                var nome = sc.Scatola.Substring(0, 8);

                var count = Regex.Matches(sc.Scatola, nome).Count;
                                
                if (count > 1)
                { 
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-error", error);
        }
    }
}
