using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UCDArch.Core.PersistanceSupport;
using NHibernate.Linq;

namespace UCDArch.Data.NHibernate
{
    public class NHibernateQueryExtensionProvider : IQueryExtensionProvider 
    {
        public IQueryable<T> Cache<T>(IQueryable<T> queryable, string region)
        {
            var query = queryable.Cacheable();

            if (region != null) query = query.CacheRegion(region);

            return query;
        }

        public IQueryable<TOriginal> Fetch<TOriginal, TRelated>(IQueryable<TOriginal> queryable, Expression<Func<TOriginal, TRelated>> relationshipProperty, params Expression<Func<TRelated, TRelated>>[] thenFetchRelationship)
        {
            var ret = queryable.Fetch(relationshipProperty);

            return thenFetchRelationship.Aggregate(ret, (current, fetchExpression) => current.ThenFetch(fetchExpression));
        }

        public IQueryable<TOriginal> FetchMany<TOriginal, TRelated>(IQueryable<TOriginal> queryable, Expression<Func<TOriginal, IEnumerable<TRelated>>> relationshipCollection, params Expression<Func<TRelated, IEnumerable<TRelated>>>[] thenFetchManyRelationship)
        {
            var ret = queryable.FetchMany(relationshipCollection);

            return thenFetchManyRelationship.Aggregate(ret, (current, fetchExpression) => current.ThenFetchMany(fetchExpression));
        }

        public IEnumerable<T> ToFuture<T>(IQueryable<T> queryable)
        {
            var query = LinqExtensionMethods.ToFuture(queryable);

            return query;
        }
    }
}
