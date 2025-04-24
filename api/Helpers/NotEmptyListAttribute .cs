using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class NotEmptyListAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not IList list || list.Count == 0)
            {
                return new ValidationResult($"{validationContext.DisplayName} cannot be empty");
            }
            return ValidationResult.Success;
        }
    }
}