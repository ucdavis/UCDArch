// using System;

// namespace UCDArch.Web.Attributes
// {
//     /// <summary>
//     /// Validate the XSRF token stored in the X-XSRF-TOKEN header.
//     /// If the header doesn't exist, look for the XSRF token in the form post.
//     /// </summary>
//     [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
//     public class ValidateAntiForgeryTokenHeaderOrFormAttribute : FilterAttribute, IAuthorizationFilter
//     {
//         private const string TokenHeaderName = "X-XSRF-TOKEN"; //Angular token, but could be anything

//         public void OnAuthorization(AuthorizationContext filterContext)
//         {
//             var token = filterContext.HttpContext.Request.Headers[TokenHeaderName];

//             if (token != null)
//             {
//                 //validate the token stored in header, so check it against the validation cookie
//                 if (filterContext.HttpContext.Request == null) { throw new ArgumentNullException("filterContext"); }

//                 var cookieToken = filterContext.HttpContext.Request.Cookies[AntiForgeryConfig.CookieName];
//                 if (cookieToken == null) { throw new HttpAntiForgeryException("Required verification token not found"); }
//                 AntiForgery.Validate(cookieToken.Value, token);
//             }
//             else
//             {
//                 //validate the token stored in form. Same as ValidateAntiForgeryTokenAttribute
//                 AntiForgery.Validate();
//             }
//         }
//     }
// }