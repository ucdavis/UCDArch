using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace UCDArch.Web.IoC
{
    public static class WindsorExtensions
    {
        /// <summary>
        /// Searches for the first interface found associated with the 
        /// <see cref="ServiceDescriptor" /> which is not generic and which 
        /// is found in the specified namespace.
        /// </summary>
        public static BasedOnDescriptor FirstNonGenericCoreInterface(this ServiceDescriptor descriptor, string interfaceNamespace)
        {
            return descriptor.Select(
                delegate(Type type, Type[] baseType)
                    {
                        var interfaces =
                            type.GetInterfaces().Where(
                                t => t.IsGenericType == false && t.Namespace.StartsWith(interfaceNamespace));

                        if (interfaces.Count() > 0)
                        {
                            return new[] { interfaces.ElementAt(0) };
                        }

                        return null;
                    });
        }

        public static IWindsorContainer RegisterController<T>(this IWindsorContainer container) where T : Microsoft.AspNetCore.Mvc.Controller 
        {
            container.RegisterControllers(typeof(T));
            return container;
        }

        public static IWindsorContainer RegisterControllers(this IWindsorContainer container, params Type[] controllerTypes)
        {
            foreach (var type in controllerTypes)
            {
                if (IsController(type))
                {
                    container.Register(Component.For(type).Named(type.FullName.ToLower()).LifeStyle.Is(LifestyleType.Transient));
                }
            }

            return container;
        }

        public static IWindsorContainer RegisterControllers(this IWindsorContainer container, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                container.RegisterControllers(assembly.GetExportedTypes());
            }
            return container;
        }

        /// <summary>
        /// Determines whether the specified type is a controller
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <see cref="http://mvccontrib.codeplex.com/SourceControl/changeset/view/1cba8c95cdc2#src%2fMVCContrib%2fControllerExtensions.cs"/>
        /// <returns>True if type is a controller, otherwise false</returns>
        private static bool IsController(Type type)
        {
            return type != null
                   && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                   && !type.IsAbstract
                   && typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type);
        }
    }
}