using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Loby.AspNetCore.Extensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Adds the specified <paramref name="errorMessages"/> to the <see cref="ModelStateEntry.Errors"/> 
        /// instance without any key specification. If the maximum number of allowed errors has already been 
        /// recorded, ensures that a <see cref="TooManyModelErrorsException"/> exception is recorded instead.
        /// </summary>
        /// <param name="modelState">
        /// An instance of <see cref="ModelStateDictionary"/>.
        /// </param>
        /// <param name="errorMessages">
        /// The error messages to add.
        /// </param>
        /// <returns>
        /// Returns the current instance of <see cref="ModelStateDictionary"/> that error messages are added to it.
        /// </returns>
        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, IEnumerable<string> errorMessages)
        {
            if(modelState == null)
            {
                throw new NullReferenceException(nameof(modelState));
            }

            if (errorMessages == null)
            {
                throw new NullReferenceException(nameof(errorMessages));
            }

            foreach (var error in errorMessages)
            {
                modelState.AddModelError(string.Empty, error);
            }

            return modelState;
        }

        /// <summary>
        /// Returns all distinct error messages exist in the current <see cref="ModelStateDictionary"/> instance.
        /// </summary>
        /// <param name="modelState">
        /// An instance of <see cref="ModelStateDictionary"/>.
        /// </param>
        /// <returns>
        /// Returns a collection that containing all model state distinct error messages.
        /// </returns>
        public static IEnumerable<string> GetErrorMessages(this ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new NullReferenceException(nameof(modelState));
            }

            return modelState.SelectMany(x => x.Value.Errors.Select(x => x.ErrorMessage)).Distinct();
        }
    }
}
