using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Loby.AspNetCore.Services
{
    /// <summary>
    /// A service that provide rendering for pages with the Razor syntax.
    /// </summary>
    public class RazorViewRenderService : IRazorViewRenderService
    {
        private readonly IRazorViewEngine _razorEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RazorViewRenderService(IRazorViewEngine razorEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            if (razorEngine == null)
            {
                throw new ArgumentNullException(nameof(razorEngine));
            }

            if (tempDataProvider == null)
            {
                throw new ArgumentNullException(nameof(tempDataProvider));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _razorEngine = razorEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
            _httpContextAccessor = httpContextAccessor;
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
            return RenderAsync(viewName, string.Empty);
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
            var actionContext = GetActionContext();

            var viewEngineResult = _razorEngine.FindView(actionContext, viewName, isMainPage: false);

            if (!viewEngineResult.Success)
            {
                viewEngineResult = _razorEngine.GetView("~/", viewName, isMainPage: false);

                if (!viewEngineResult.Success)
                {
                    throw new FileNotFoundException($"Any view with name '{viewName}' couldn't be found.");
                }
            }

            var view = viewEngineResult.View;
            var tempDataDictionary = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model,
            };

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(actionContext, view, viewDataDictionary, tempDataDictionary, output, new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        /// <summary>
        /// Creates or gets context object for execution of action which has been 
        /// selected as part of an HTTP request.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ActionContext"/> that is used for rendering views.
        /// </returns>
        private ActionContext GetActionContext()
        {
            var httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext != null)
            {
                return new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
            }

            httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
