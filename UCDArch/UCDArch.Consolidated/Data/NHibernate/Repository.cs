using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace UCDArch.Data.NHibernate
{
    /// <summary>
    /// Since nearly all of the domain objects you create will have a type of int Id, this 
    /// most freqently used base GenericDao leverages this assumption.  If you want an entity
    /// with a type other than int, such as string, then use ///TODO
    /// </summary>
    public class Repository<T> : RepositoryWithTypedId<T, int>, IRepository<T> where T : ValidatableObject
    { }

    public class Repository : IRepository
    {
        public IRepository<T> OfType<T>() where T : ValidatableObject
        {
            return new Repository<T>();
        }
    }

    /// <summary>
    /// Provides a fully loaded DAO which may be created in a few ways including:
    /// * Direct instantiation; e.g., new GenericDao<Customer, string>
    /// * Spring configuration; e.g., <object id="CustomerDao" type="SharpArch.Data.NHibernateSupport.GenericDao&lt;CustomerAlias, string>, SharpArch.Data" autowire="byName" />
    /// </summary>
    public class RepositoryWithTypedId<T, IdT> : IRepositoryWithTypedId<T, IdT> where T : ValidatableObject
    {
        protected virtual ISession Session
        {
            get
            {
                return NHibernateSessionManager.Instance.GetSession();
            }
        }

        public virtual IDbContext DbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = new DbContext();
                }

                return _dbContext;
            }
        }

        /// <summary>
        /// Returns an IQueryable<typeparam name="T">DomainObject type</typeparam> for querying with Linq
        /// </summary>
        public virtual IQueryable<T> Queryable
        {
            get
            {
                return Session.Query<T>();
            }
        }
        
        /// <summary>
        /// Loads an object with the given Id.  May load a proxy object.
        /// </summary>
        public virtual T GetById(IdT id)
        {
            return Session.Load<T>(id);
        }

        /// <summary>
        /// Gets an object from the database.  If no object is found, null is returned
        /// </summary>
        public virtual T GetNullableById(IdT id)
        {
            return Session.Get<T>(id);
        }

        /// <summary>
        /// Returns all of the items of a given type.
        /// </summary>
        public IList<T> GetAll()
        {
            ICriteria criteria = Session.CreateCriteria(typeof (T));
            return criteria.List<T>();
        }


        /// <summary>
        /// Overload of getall that will sort the results by the properties given
        /// </summary>
        /// <param name="propertyName">Property to sort by</param>
        /// <param name="ascending">Sort direction</param>
        /// <returns>Sorted list of type <typeparamref name="T"/></returns>
        public IList<T> GetAll(string propertyName, bool ascending)
        {
            ICriteria criteria = Session.CreateCriteria(typeof (T))
                .AddOrder(new Order(propertyName, ascending));

            return criteria.List<T>();
        }

        /// <summary>
        /// Saves/updates an entity -- only valid entites will be persisted, else an exception will be thrown
        /// </summary>
        /// <param name="entity">Entity to be persisted</param>
        public void EnsurePersistent(T entity)
        {
            EnsurePersistent(entity, false);
        }

        /// <summary>
        /// Overload of EnsurePersistent which forces a save call be performed 
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <param name="forceSave">Force a save, not an update.  Use only with assigned Ids</param>
        public void EnsurePersistent(T entity, bool forceSave)
        {
            EnsurePersistent(entity, forceSave, true /* Flush changes by default */);
        }

        /// <summary>
        /// Overload of EnsurePersistent which optionally forces a save call be performed and can force changes to be flushed
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <param name="forceSave">Force a save, not an update.  Use only with assigned Ids</param>
        /// <param name="flushChanges">Whether or not to flush changes</param>
        public void EnsurePersistent(T entity, bool forceSave, bool flushChanges)
        {
            if (entity.IsValid()) //Only save valid entities
            {
                if (forceSave)
                {
                    Session.Save(entity);
                }
                else
                {
                    Session.SaveOrUpdate(entity);
                }

                if (flushChanges)
                {
                    Session.Flush(); //Flush the changes to the DB
                }
            }
            else
            {
                //For non-valid entities, throw an exception giving validation errors
                var errorMessage = new StringBuilder();

                errorMessage.AppendLine(string.Format("Object of type {0} could not be persisted\n\n", typeof(T)));

                var validationErrors = entity.ValidationResults();

                foreach (var error in validationErrors)
                {
                    errorMessage.AppendLine(string.Format("{0}: {1}", error.PropertyName, error.Message));
                }

                throw new ApplicationException(errorMessage.ToString());
            }
        }

        public void Remove(T entity)
        {
            Remove(entity, true /*flush changes by default*/);
        }

        public void Remove(T entity, bool flushChanges)
        {
            Session.Delete(entity);

            if (flushChanges) Session.Flush();
        }

        private IDbContext _dbContext;
    }
}