using System;
using Loby.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Loby.AspNetCore.Extensions.DependencyInjection
{
    public static class LobyServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a default implementation for the <see cref="IControllerDiscoveryService"/> 
        /// that provide some information about all controllers and actions.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// The service collection.
        /// </returns>
        public static IServiceCollection AddControllerDiscoveryService(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<IControllerDiscoveryService, ControllerDiscoveryService>();

            return services;
        }

        /// <summary>
        /// Adds a default implementation for the <see cref="IRazorViewRenderService"/> that 
        /// provide rendering for pages with the Razor syntax.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// The service collection.
        /// </returns>
        public static IServiceCollection AddRazorViewRenderService(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddActionContextAccessor();
            services.TryAddScoped<IRazorViewRenderService, RazorViewRenderService>();

            return services;
        }
    }
}