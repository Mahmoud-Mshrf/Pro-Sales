using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Filters
{
    public class CustomValidationResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var errors = new Dictionary<string, List<string>>();

            foreach (var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;
                var errorsList = keyModelStatePair.Value.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                errors.Add(key, errorsList);
            }

            var customErrorResponse = new
            {
                errors = errors
            };

            context.Result = new BadRequestObjectResult(customErrorResponse);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Implementation not needed for this example
        }
    }
}
