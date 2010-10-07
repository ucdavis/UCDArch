using System;
using System.Web.Mvc;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;

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

        /// <summary>
        /// Look for defined transactional base attrs on the action and controller.  If we find them on either
        /// then return true (should delegate)
        /// </summary>
        private static bool ShouldDelegateTransactionSupport(ActionExecutingContext context)
        {
            var hasTransactionalControllerAttrs =
                context.ActionDescriptor.ControllerDescriptor.IsDefined(typeof (TransactionalActionBaseAttribute), false);
            
            var hasTransactionalMethodAttrs = context.ActionDescriptor.IsDefined(typeof (TransactionalActionBaseAttribute), false);

            return hasTransactionalControllerAttrs || hasTransactionalMethodAttrs;
        }
    }
}