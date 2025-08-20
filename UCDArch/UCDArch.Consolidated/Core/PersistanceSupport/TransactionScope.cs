using System;

namespace UCDArch.Core.PersistanceSupport
{
    public class TransactionScope : IDisposable
    {
        private readonly IDbContext _dbContext;
        
        public TransactionScope()
        {
            _dbContext = SmartServiceLocator<IDbContext>.GetService();

            _dbContext.BeginTransaction();
        }

        public void RollBackTransaction()
        {
            _dbContext.RollbackTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.CommitTransaction();
        }

        public bool HasOpenTransaction
        {
            get { return _dbContext.IsActive; }
        }

        public void Dispose()
        {
            if (_dbContext.IsActive) //Rollback the transaction if it has not been committed 
            {
                _dbContext.RollbackTransaction();
            }
        }
    }
}