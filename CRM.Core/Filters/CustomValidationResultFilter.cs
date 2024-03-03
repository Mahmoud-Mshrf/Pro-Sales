using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Filters
{
    public class CustomValidationResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var orderedErrors = new List<string>();

                var propertiesOrder = context
                    .ActionDescriptor
                    .Parameters
                    .SelectMany(p => p.ParameterType.GetProperties())
                    .Select(prop => new
                    {
                        PropertyName = prop.Name,
                        Order = GetValidationErrorOrder(prop)
                    })
                    .OrderBy(o => o.Order)
                    .Select(o => o.PropertyName)
                    .ToList();

                foreach (var propertyName in propertiesOrder)
                {
                    if (context.ModelState.TryGetValue(propertyName, out var propertyErrors))
                    {
                        orderedErrors.AddRange(propertyErrors.Errors.Select(error => error.ErrorMessage));
                    }
                }

                var result = new ObjectResult(new { errors = orderedErrors })
                {
                    StatusCode = 400,
                };

                context.Result = result;
            }
        }

        private int GetValidationErrorOrder(PropertyInfo property)
        {
            var validationErrorOrderAttribute = property.GetCustomAttribute<ValidationErrorOrderAttribute>();
            return validationErrorOrderAttribute?.Order ?? int.MaxValue;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // No action needed after the action method executes
        }
    }
}
