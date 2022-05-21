using System;
using System.Collections.Generic;
using Loby.AspNetCore.Services.Models;

namespace Loby.AspNetCore.Services
{
    public interface IControllerDiscoveryService
    {
        /// <summary>
        /// A list of all controllers and action methods.
        /// </summary>
        ICollection<ControllerInfo> Controllers { get; }

        /// <summary>
        /// A list of all secured controllers and action methods.
        /// </summary>
        ICollection<ControllerInfo> SecuredControllers { get; }

        /// <summary>
        /// Get all controllers and actions secured and have the 
        /// specified <paramref name="policyName"/>.
        /// </summary>
        /// <param name="policyName">
        /// An string representing a policy name.
        /// </param>
        IReadOnlyList<ControllerInfo> GetSecuredControllers(string policyName);
    }
}
