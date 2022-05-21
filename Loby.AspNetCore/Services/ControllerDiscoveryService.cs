using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Loby.AspNetCore.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Loby.AspNetCore.Services
{
    /// <summary>
    /// A discovery service that provide some information about all controllers and actions.
    /// </summary>
    public class ControllerDiscoveryService : IControllerDiscoveryService
    {
        /// <summary>
        /// A list of all controllers and action methods.
        /// </summary>
        public ICollection<ControllerInfo> Controllers { get; protected set; }

        /// <summary>
        /// A list of all secured controllers and action methods.
        /// </summary>
        public ICollection<ControllerInfo> SecuredControllers { get; protected set; }

        /// <summary>
        /// Provides the currently cached collection of <see cref="ActionDescriptor"/>.
        /// </summary>
        protected readonly IActionDescriptorCollectionProvider _discoveryProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="ControllerDiscoveryService"/>.
        /// </summary>
        /// <param name="actionDescriptorCollectionProvider"></param>
        public ControllerDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            if (actionDescriptorCollectionProvider == null)
            {
                throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));
            }

            _discoveryProvider = actionDescriptorCollectionProvider;

            InitializeControllers();
            InitializeSecuredControllers();
        }

        /// <summary>
        /// Initialize the <see cref="Controllers"/>.
        /// </summary>
        protected virtual void InitializeControllers()
        {
            var lastControllerName = string.Empty;
            ControllerInfo currentController = null;
            Controllers = new List<ControllerInfo>();
            var actionDescriptors = _discoveryProvider.ActionDescriptors.Items;

            foreach (var actionDescriptor in actionDescriptors)
            {
                if (!(actionDescriptor is ControllerActionDescriptor descriptor))
                {
                    continue;
                }

                var actionMethodInfo = descriptor.MethodInfo;
                var controllerTypeInfo = descriptor.ControllerTypeInfo;

                if (lastControllerName != descriptor.ControllerName)
                {
                    currentController = new ControllerInfo
                    {
                        Actions = new List<ControllerActionInfo>(),
                        Name = descriptor.ControllerName.ToLower(),
                        AreaName = GetAreaName(controllerTypeInfo),
                        Attributes = GetAttributes(controllerTypeInfo),
                        IsSecured = IsSecured(controllerTypeInfo, actionMethodInfo),
                        DisplayName = GetDisplayName(controllerTypeInfo) ?? descriptor.ControllerName,
                    };

                    Controllers.Add(currentController);

                    lastControllerName = descriptor.ControllerName;
                }

                var currentAction = new ControllerActionInfo
                {
                    ControllerId = currentController.Id,
                    Name = descriptor.ActionName.ToLower(),
                    Attributes = GetAttributes(actionMethodInfo),
                    IsSecured = IsSecured(controllerTypeInfo, actionMethodInfo),
                    DisplayName = GetDisplayName(actionMethodInfo.GetType()) ?? descriptor.ActionName,
                };

                // Distinc duplicated actions by id
                if (IsControllerHasAction(currentController, currentAction.Id))
                {
                    currentController.Actions.Add(currentAction);
                }
            }
        }

        /// <summary>
        /// Initialize the <see cref="SecuredControllers"/>.
        /// </summary>
        protected virtual void InitializeSecuredControllers()
        {
            SecuredControllers = new List<ControllerInfo>();

            foreach (var controller in Controllers)
            {
                if (controller.IsSecured)
                {
                    controller.Actions = controller.Actions.Where(x => x.IsSecured).ToList();

                    SecuredControllers.Add(controller);
                }
            }
        }

        /// <summary>
        /// Get all controllers and actions secured by <see cref="AuthorizeAttribute"/> and 
        /// have the specified <paramref name="policyName"/>.
        /// </summary>
        /// <param name="policyName">
        /// An string representing a policy name.
        /// </param>
        /// <returns>
        /// returns a <see cref="ICollection{ControllerInfo}"/> containing all controllers and 
        /// action methods secured by <see cref="AuthorizeAttribute"/> and have the specified 
        /// <paramref name="policyName"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// policyName is null.
        /// </exception>
        public virtual IReadOnlyList<ControllerInfo> GetSecuredControllers(string policyName)
        {
            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            var securedByPolicyControllers = new List<ControllerInfo>();

            foreach (var controller in SecuredControllers)
            {
                controller.Actions = controller.Actions
                    .Where(action =>
                        ContainsPolicy(action.Attributes, policyName) ||
                        ContainsPolicy(controller.Attributes, policyName))
                    .ToList();

                securedByPolicyControllers.Add(controller);
            }

            return securedByPolicyControllers;
        }

        #region utilities

        private bool IsControllerHasAction(ControllerInfo controller, string actionId)
        {
            return !controller.Actions.Any(x => x.Id == actionId);
        }

        protected bool ContainsPolicy(ICollection<Attribute> attributes, string policyName)
        {
            return attributes
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault()?.Policy == policyName;
        }

        protected string GetDisplayName(Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }

        protected string GetAreaName(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue.ToLower();
        }

        protected List<Attribute> GetAttributes(MemberInfo actionMemberInfo)
        {
            var attributes = actionMemberInfo
                .GetCustomAttributes(inherit: true)
                .Where(x =>
                {
                    var attributeNamespace = x.GetType().Namespace;

                    var result = attributeNamespace != typeof(CompilerGeneratedAttribute).Namespace &&
                                 attributeNamespace != typeof(DebuggerStepThroughAttribute).Namespace;

                    return result;
                });

            return attributes.Cast<Attribute>().ToList();
        }

        protected bool IsSecured(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
        {
            var actionAuthorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true);
            var controllerAuthorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true);

            if (actionAuthorizeAttribute != null || controllerAuthorizeAttribute != null)
            {
                return true;
            }

            return false;
        }

        #endregion;
    }
}
