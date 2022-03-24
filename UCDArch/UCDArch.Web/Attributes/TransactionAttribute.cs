using System;
using Microsoft.AspNetCore.Mvc.Filters;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;

namespace UCDArch.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TransactionAttribute : TransactionalActionBaseAttribute
    {
        private IDbContext _dbContext;

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
            DbContext.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
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
    }
}