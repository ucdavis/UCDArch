using System.Security.Principal;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;

namespace UCDArch.Web.Controller
{
    [UseTransactionsByDefault]
    [UseAntiForgeryTokenOnPostByDefault]
    public class SuperController : System.Web.Mvc.Controller
    {
        public IRepository Repository { get; set; } //General repository set through DI

        public string Message
        {
            get { return TempData[TEMP_DATA_MESSAGE_KEY] as string; }
            set { TempData[TEMP_DATA_MESSAGE_KEY] = value; }
        }

        public IPrincipal CurrentUser
        {
            get
            {
                return ControllerContext.HttpContext.User;
            }
        }

        private const string TEMP_DATA_MESSAGE_KEY = "Message";
    }
}