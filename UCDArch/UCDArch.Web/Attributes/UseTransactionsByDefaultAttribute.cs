using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace UCDArch.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UseTransactionsByDefaultAttribute : ActionFilterAttribute
    {
        private IDbContext _dbContext;
        private bool _delegateTransactionSupport;

        public IDbContext DbContext
        {
            get
            {
                if (_dbContext == null) _dbContext = SmartServiceLocator<IDbContext>.GetService();

                return _dbContext;
            }
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _delegateTransactionSupport = ShouldDelegateTransactionSupport(filterContext);

            if (_delegateTransactionSupport) return;

            DbContext.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (_delegateTransactionSupport) return;

            if (DbContext.IsActive)
            {
                if (filterContext.Exception == null)
                {
                    DbContext.CommitTransaction();
                }
                else
                {
                    DbContext.RollbackTransaction();
                }
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //We might want to close sessions even if we aren't delegated transaction support
            if (_delegateTransactionSupport) return;

            _dbContext.CloseSession();
        }

        /// <summary>
        /// Look for defined transactional base attrs on the action and controller.  If we find them on either
        /// then return true (should delegate)
        /// </summary>
        private static bool ShouldDelegateTransactionSupport(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                return controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                .Any(a => a.GetType().Equals(typeof(TransactionalActionBaseAttribute)));

            }

            return false;
        }
    }
}