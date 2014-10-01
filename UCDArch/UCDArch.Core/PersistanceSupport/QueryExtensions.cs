using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UCDArch.Core.PersistanceSupport
{
    public static class QueryExtensions
    {
        private static readonly IQueryExtensionProvider QueryExtensionProvider = SmartServiceLocator<IQueryExtensionProvider>.GetService();

        public static IQueryable<TOriginal> Fetch<TOriginal, TRelated>(this IQueryable<TOriginal> queryable,
                                                                        Expression<Func<TOriginal, TRelated>> relationshipProperty,
                                                                        params Expression<Func<TRelated, object>>[] thenFetchRelationship)
        {
            return QueryExtensionProvider.Fetch(queryable, relationshipProperty, thenFetchRelationship);
        }

        public static IQueryable<TOriginal> FetchMany<TOriginal, TRelated>(this IQueryable<TOriginal> queryable,
                                                                            Expression<Func<TOriginal, IEnumerable<TRelated>>> relationshipCollection,
                                                                            params Expression<Func<TRelated, IEnumerable<TRelated>>>[] thenFetchManyRelationship)
        {
            return QueryExtensionProvider.FetchMany(queryable, relationshipCollection, thenFetchManyRelationship);
        }

        public static IQueryable<T> Cache<T>(this IQueryable<T> queryable, string region = null)
        {
            return QueryExtensionProvider.Cache(queryable, region);
        }

        public static IEnumerable<T> ToFuture<T>(this IQueryable<T> queryable)
        {
            return QueryExtensionProvider.ToFuture(queryable);
        }
    }
}