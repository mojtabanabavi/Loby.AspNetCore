using System;
using System.IO;
using System.Threading.Tasks;

namespace Loby.AspNetCore.Services
{
    public interface IRazorViewRenderService
    {
        /// <summary>
        /// Renders the view with the specified name or path.
        /// </summary>
        /// <param name="viewName">
        /// The name or path of the view that is rendered to the response.
        /// </param>
        /// <returns>
        /// An string that represent the rendered view.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Any view with name '<paramref name="viewName"/>' couldn't be found.
        /// </exception>
        Task<string> RenderAsync(string viewName);

        /// <summary>
        /// Renders the view with the specified name or path and model.
        /// </summary>
        /// <param name="viewName">
        /// The name or path of the view that is rendered to the response.
        /// </param>
        /// <param name="model">
        /// An object that is used to pass information to view.
        /// </param>
        /// <returns>
        /// An string that represent the rendered view.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Any view with name '<paramref name="viewName"/>' couldn't be found.
        /// </exception>
        Task<string> RenderAsync(string viewName, object model);
    }
}