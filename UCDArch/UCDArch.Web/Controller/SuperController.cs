using System.Security.Principal;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Web.Attributes;

namespace UCDArch.Web.Controller
{
    [UseTransactionsByDefault]
    [UseAntiForgeryTokenOnPostByDefault]
    public class SuperController : System.Web.Mvc.Controller
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

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            NHibernateSessionManager.Instance.CloseSession();
            base.OnResultExecuted(filterContext);
        }
    }
}