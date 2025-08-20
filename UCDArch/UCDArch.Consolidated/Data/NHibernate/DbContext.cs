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

        private ITransaction _currentTransaction;

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
            if (_currentTransaction != null && _currentTransaction.IsActive)
            {
                // Handle nested transaction or throw exception
                throw new InvalidOperationException("A transaction is already active.");
            }
            _currentTransaction = Session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_currentTransaction != null && _currentTransaction.IsActive)
            {
                _currentTransaction.Commit();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
            else
            {
                var tx = Session.GetCurrentTransaction();
                if (tx != null && tx.IsActive)
                {
                    tx.Commit();
                }
            }
        }

        public void RollbackTransaction()
        {
            if (_currentTransaction != null && _currentTransaction.IsActive)
            {
                _currentTransaction.Rollback();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
            else
            {
                var tx = Session.GetCurrentTransaction();
                if (tx != null && tx.IsActive)
                {
                    tx.Rollback();
                }
            }
        }

        public void CloseSession()
        {
            NHibernateSessionManager.Instance.CloseSession();
        }

        public bool IsActive
        {
            get 
            { 
                return (_currentTransaction?.IsActive ?? false) || 
                       (Session.GetCurrentTransaction()?.IsActive ?? false); 
            }
        }
    }
}