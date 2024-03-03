using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Custom_Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidationErrorOrderAttribute : ValidationAttribute
    {
        public int Order { get; }

        public ValidationErrorOrderAttribute(int order)
        {
            Order = order;
        }
        override protected ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}
