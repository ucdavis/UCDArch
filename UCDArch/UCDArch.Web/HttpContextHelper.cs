using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;

namespace UCDArch.Web
{
    /// <summary>
    /// Temporary helper class for retrieving the current <see cref="HttpContext"/> . This temporary
    /// workaround should be removed in the future and <see cref="HttpContext"/> should be retrieved
    /// from the current controller, middleware, or page instead.
    ///
    /// If working in another component, the current <see cref="HttpContext"/> can be retrieved from an <see cref="IHttpContextAccessor"/>
    /// retrieved via dependency injection.
    /// </summary>
    internal static class HttpContextHelper
    {
        private const string Message = "Prefer accessing HttpContext via injection";

        /// <summary>
        /// Gets the current <see cref="HttpContext"/>. Returns <c>null</c> if there is no current <see cref="HttpContext"/>.
        /// </summary>
        [Obsolete(Message, error: false, DiagnosticId = "HttpContextCurrent", UrlFormat = "https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context")]
        public static HttpContext Current => HttpContextAccessor.HttpContext;

        internal static IHttpContextAccessor HttpContextAccessor = new HttpContextAccessor();
    }
}
