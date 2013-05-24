namespace UCDArch.Core.PersistanceSupport
{
    public interface IDbContext
    {
        void CommitChanges();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        bool IsActive { get; }
        void CloseSession();
    }
}