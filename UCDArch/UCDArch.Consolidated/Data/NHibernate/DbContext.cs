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
                throw new InvalidOperationException("A transaction is already active. Nested transactions are not supported.");
            }
            
            // Check if there's already an active session-level transaction
            var sessionTx = Session.GetCurrentTransaction();
            if (sessionTx != null && sessionTx.IsActive)
            {
                throw new InvalidOperationException("A session-level transaction is already active. Cannot start a new transaction.");
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
                throw new InvalidOperationException("No active transaction to commit. Call BeginTransaction() first.");
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
                throw new InvalidOperationException("No active transaction to rollback. Call BeginTransaction() first.");
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
                return _currentTransaction?.IsActive ?? false; 
            }
        }
    }
}