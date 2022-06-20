using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Loby.AspNetCore.Services
{
    /// <summary>
    /// A service that provide rendering for pages with the Razor syntax.
    /// </summary>
    public class RazorViewRenderService : IRazorViewRenderService
    {
        private readonly ActionContext _actionContext;
        private readonly IRazorViewEngine _razorEngine;
        private readonly ITempDataProvider _tempDataProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="RazorViewRenderService"/>.
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="tempDataProvider"></param>
        /// <param name="actionContextAccessor"></param>
        public RazorViewRenderService(IRazorViewEngine razorEngine, ITempDataProvider tempDataProvider, IActionContextAccessor actionContextAccessor)
        {
            if (razorEngine == null)
            {
                throw new ArgumentNullException(nameof(razorEngine));
            }

            if (tempDataProvider == null)
            {
                throw new ArgumentNullException(nameof(tempDataProvider));
            }

            if (actionContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(actionContextAccessor));
            }

            _razorEngine = razorEngine;
            _tempDataProvider = tempDataProvider;
            _actionContext = actionContextAccessor.ActionContext;
        }

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
        public Task<string> RenderAsync(string viewName)
        {
            return RenderAsync(viewName, default);
        }

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
        public async Task<string> RenderAsync(string viewName, object model)
        {
            var viewEngineResult = _razorEngine.FindView(_actionContext, viewName, isMainPage: false);

            if (!viewEngineResult.Success)
            {
                viewEngineResult = _razorEngine.GetView("~/", viewName, isMainPage: false);

                if (!viewEngineResult.Success)
                {
                    throw new FileNotFoundException($"Any view with name '{viewName}' couldn't be found.");
                }
            }

            var view = viewEngineResult.View;
            var tempDataDictionary = new TempDataDictionary(_actionContext.HttpContext, _tempDataProvider);
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model,
            };

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(_actionContext, view, viewDataDictionary, tempDataDictionary, output, new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }
    }
}
