using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;

namespace UCDArch.Web.Controller
{
    [UseTransactionsByDefault]
    [AutoValidateAntiforgeryToken]
    public class SuperController : Microsoft.AspNetCore.Mvc.Controller
    {
        private const string TempDataMessageKey = "Message";

        public IRepository Repository { get; set; } //General repository set through DI
        
        public string Message
        {
            get { return TempData[TempDataMessageKey] as string; }
            set { TempData[TempDataMessageKey] = value; }
        }

        public IPrincipal CurrentUser
        {
            get
            {
                return ControllerContext.HttpContext.User;
            }
        }
    }
}