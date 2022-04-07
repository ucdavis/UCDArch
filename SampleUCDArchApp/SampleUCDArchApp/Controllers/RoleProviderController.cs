using System.Web.Security;

namespace SampleUCDArchApp.Controllers
{
    /// <summary>
    /// Controller for testing out the service role provider
    /// </summary>
    public class RoleProviderController : Microsoft.AspNetCore.Mvc.Controller
    {
        //
        // GET: /RoleProvider/
        /// <summary>
        /// This shows some examples of using the Role provider directly.  Of course, most of the time
        /// you will use the [Authorize()] attribute for security
        /// </summary>
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            var isUserInRole = Roles.IsUserInRole("user", "role");
            var roles = Roles.GetAllRoles();
            var users = Roles.GetUsersInRole("role");

            return Content("User count: #" + users.Length);
        }

    }
}
