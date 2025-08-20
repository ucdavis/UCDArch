// Direct copy of the WindsorControllerFactory.cs in MVCContrib::
// http://mvccontrib.codeplex.com/SourceControl/changeset/view/b7039b7291cf#src%2fMvcContrib.Castle%2fWindsorControllerFactory.cs

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Castle.Windsor;

namespace UCDArch.Web.IoC
{
    /// <summary>
    /// Controller Factory class for instantiating controllers using the Windsor IoC container.
    /// </summary>
    public class WindsorControllerFactory : IControllerFactory
    {
        private readonly IWindsorContainer _container;

        /// <summary>
        /// Creates a new instance of the <see cref="WindsorControllerFactory"/> class.
        /// </summary>
        /// <param name="container">The Windsor container instance to use when creating controllers.</param>
        public WindsorControllerFactory(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this._container = container;
        }

        public object CreateController(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ActionDescriptor == null)
            {
                throw new InvalidOperationException("No ActionDescriptor found");
            }

            var controllerType = Type.GetType(context.ActionDescriptor.ControllerTypeInfo.AssemblyQualifiedName);

            if (controllerType == null)
            {
                throw new InvalidOperationException(string.Format("The controller for path '{0}' could not be found or it does not inherit Microsoft.AspNetCore.Mvc.Controller.", context.HttpContext.Request.Path));
            }

            return (Microsoft.AspNetCore.Mvc.Controller)this._container.Resolve(controllerType);
        }

        public void ReleaseController(ControllerContext context, object controller)
        {
            var disposable = controller as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }

            this._container.Release(controller);
        }
    }
}