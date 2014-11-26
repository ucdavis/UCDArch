using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace UCDArch.Web.Authentication
{
    public static class CasMvc
    {
        private const string StrCasUrl = "https://cas.ucdavis.edu/cas/";
        private const string StrTicket = "ticket";
        private const string StrReturnUrl = "ReturnURL";

        public static string GetReturnUrl()
        {
            return HttpContext.Current.Request.QueryString[StrReturnUrl];
        }

        public static ActionResult LoginAndRedirect(Action<string> handleUserId = null)
        {
            var actionResult = Login(handleUserId); //Do the CAS Login

            return actionResult ?? new ViewResult();
        }

        /// <summary>
        /// Login to the campus CAS system and integrate with MVC results
        /// </summary>
        /// <returns></returns>
        public static ActionResult Login(Action<string> handleUserId = null)
        {
            // get the context from the source
            var context = HttpContext.Current;

            // try to load a valid ticket
            HttpCookie validCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket validTicket = null;

            // check to make sure cookie is valid by trying to decrypt it
            if (validCookie != null)
            {
                try
                {
                    validTicket = FormsAuthentication.Decrypt(validCookie.Value);
                }
                catch
                {
                    validTicket = null;
                }
            }

            // if user is unauthorized and no validTicket is defined then authenticate with cas
            // if (context.Response.StatusCode == 0x191 && (validTicket == null || validTicket.Expired))
            if (validTicket == null || validTicket.Expired)
            {
                // build query string but strip out ticket if it is defined
                string query = context.Request.QueryString.AllKeys.Where(
                    key => String.Compare(key, StrTicket, StringComparison.OrdinalIgnoreCase) != 0)
                    .Aggregate("", (current, key) => current + ("&" + key + "=" + context.Request.QueryString[key]));

                // replace 1st character with ? if query is not empty
                if (!string.IsNullOrEmpty(query))
                {
                    query = "?" + query.Substring(1);
                }

                // get ticket & service
                string ticket = context.Request.QueryString[StrTicket];
                string service = context.Server.UrlEncode(context.Request.Url.GetLeftPart(UriPartial.Path) + query);

                // if ticket is defined then we assume they are coming from CAS
                if (!string.IsNullOrEmpty(ticket))
                {
                    // validate ticket against cas
                    var sr = new StreamReader(new WebClient().OpenRead(StrCasUrl + "validate?ticket=" + ticket + "&service=" + service));

                    // parse text file
                    if (sr.ReadLine() == "yes")
                    {
                        // get kerberos id
                        string kerberos = sr.ReadLine();

                        if (handleUserId != null)
                        {
                            handleUserId(kerberos);
                        }
                        else
                        {
                            // set forms authentication ticket
                            FormsAuthentication.SetAuthCookie(kerberos, false);
                        }

                        string returnUrl = GetReturnUrl();

                        return !string.IsNullOrEmpty(returnUrl)
                            ? new RedirectResult(returnUrl)
                            : new RedirectResult(FormsAuthentication.DefaultUrl);
                    }
                }

                // ticket doesn't exist or is invalid so redirect user to CAS login
                return new RedirectResult(StrCasUrl + "login?service=" + service);
            }

            //User already has a valid ticket. This likely means they are unauthorized because they were kicked back to the login page.
            return null;
        }
    }
}