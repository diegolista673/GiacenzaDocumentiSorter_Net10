using GiacenzaSorterRm.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace GiacenzaSorterRm.AppCode
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _property;
        private readonly string _value;

        public RequiredIfAttribute(string property, string value)
        {
            _property = property;
            _value = value;
        }

        /// <summary>
        /// rende obbligatorio il campo password se l'utente è un utente esterno, se postel lo rende valido se non utilizzato
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            var property = validationContext.ObjectType.GetProperty(_property);
            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var valueProperty = (string)property.GetValue(validationContext.ObjectInstance);

            if (string.IsNullOrEmpty(valueProperty))
                return new ValidationResult(ErrorMessage);

            if (valueProperty.ToLower() != _value)
            {
                var passwordProperty = validationContext.ObjectType.GetProperty("Password");
                var passwordValue = (string)passwordProperty.GetValue(validationContext.ObjectInstance);
                if (passwordValue == null)
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
