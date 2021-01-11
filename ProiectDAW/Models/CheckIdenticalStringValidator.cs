using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProiectDAW.Models
{
    public class CheckIdenticalStringValidator : ValidationAttribute
    {
        public CheckIdenticalStringValidator(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        public string[] PropertyNames { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = this.PropertyNames.Select(validationContext.ObjectType.GetProperty);
            var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null)).OfType<string>().ToList();

            for (var i = 0; i < values.Count(); i++)
            {
                if (String.Equals(value, values[i]))
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }

  
            return null;
        }
    }
}