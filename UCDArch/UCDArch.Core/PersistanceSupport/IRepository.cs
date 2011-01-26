using System.Collections.Generic;
using System.Linq;
using UCDArch.Core.DomainModel;

namespace UCDArch.Core.PersistanceSupport
{
    /// <summary>
    /// Provides a standard interface for DAOs which are data-access mechanism agnostic.
    /// 
    /// Since nearly all of the domain objects you create will have a type of int Id, this 
    /// base Idao leverages this assumption.  If you want an entity with a type 
    /// other than int, such as string, then use <see cref="IRepositoryWithTypedId{T, IdT}" />.
    /// </summary>
    public interface IRepository<T> : IRepositoryWithTypedId<T, int> where T : ValidatableObject
    { }

    public interface IRepository
    {
        IRepository<T> OfType<T>() where T : ValidatableObject;
    }

    public interface IRepositoryWithTypedId<T, IdT> where T : ValidatableObject
    {
        /// <summary>
        /// Returns the queryable set of all type T to be queried with Linq
        /// </summary>
        IQueryable<T> Queryable { get; }

        /// <summary>
        /// Loads an object with the given Id.  May load a proxy object.
        /// </summary>
        T GetById(IdT id);
        
        /// <summary>
        /// Gets an object from the database.  If no object is found, null is returned
        /// </summary>
        T GetNullableById(IdT id);

        /// <summary>
        /// Returns all of the items of a given type.
        /// </summary>
        IList<T> GetAll();

        /// <summary>
        /// Overload of getall that will sort the results by the properties given
        /// </summary>
        /// <param name="propertyName">Property to sort by</param>
        /// <param name="ascending">Sort direction</param>
        /// <returns>Sorted list of type <typeparamref name="T"/></returns>
        IList<T> GetAll(string propertyName, bool ascending);

        /// <summary>
        /// Saves/updates an entity -- only valid entites will be persisted, else an exception will be thrown
        /// </summary>
        /// <param name="entity">Entity to be persisted</param>
        void EnsurePersistent(T entity);

        /// <summary>
        /// Overload of EnsurePersistent which forces a save call be performed 
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <param name="forceSave">Force a save, not an update.  Use only with assigned Ids</param>
        void EnsurePersistent(T entity, bool forceSave);

        /// <summary>
        /// Overload of EnsurePersistent which optionally forces a save call be performed and can force changes to be flushed
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <param name="forceSave">Force a save, not an update.  Use only with assigned Ids</param>
        /// <param name="flushChanges">Whether or not to flush changes</param>
        void EnsurePersistent(T entity, bool forceSave, bool flushChanges);

        /// <summary>
        /// Removes an object
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Removes an object
        /// </summary>
        void Remove(T entity, bool flushChanges);

        /// <summary>
        /// Provides a handle to application wide DB activities such as committing any pending changes,
        /// beginning a transaction, rolling back a transaction, etc.
        /// </summary>
        IDbContext DbContext { get; }
    }
}