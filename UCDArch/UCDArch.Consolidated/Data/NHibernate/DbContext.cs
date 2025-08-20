using System;
using NHibernate;
using UCDArch.Core.PersistanceSupport;

namespace UCDArch.Data.NHibernate
{
    public class DbContext : IDbContext
    {
        private static ISession Session
        {
            get
            {
                return NHibernateSessionManager.Instance.GetSession();
            }
        }

        /// <summary>
        /// This isn't specific to any one DAO and flushes everything that has been 
        /// changed since the last commit.
        /// </summary>
        public void CommitChanges()
        {
            Session.Flush();
        }

        public void BeginTransaction()
        {
            Session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Session.GetCurrentTransaction().Commit();
        }

        public void RollbackTransaction()
        {
            Session.GetCurrentTransaction().Rollback();
        }

        public void CloseSession()
        {
            NHibernateSessionManager.Instance.CloseSession();
        }

        public bool IsActive
        {
            get { return Session.GetCurrentTransaction()?.IsActive ?? false; }
        }
    }
}