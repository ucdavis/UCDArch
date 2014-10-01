using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UCDArch.Core.PersistanceSupport
{
    public interface IQueryExtensionProvider
    {
        IQueryable<T> Cache<T>(IQueryable<T> queryable, string region = null);

        IQueryable<TOriginal> Fetch<TOriginal, TRelated>(IQueryable<TOriginal> queryable,
                                                          Expression<Func<TOriginal, TRelated>> relationshipProperty,
                                                          params Expression<Func<TRelated, object>>[] thenFetchRelationship);

        IQueryable<TOriginal> FetchMany<TOriginal, TRelated>(IQueryable<TOriginal> queryable,
                                                              Expression<Func<TOriginal, IEnumerable<TRelated>>>
                                                                  relationshipCollection,
                                                              params Expression<Func<TRelated, IEnumerable<TRelated>>>[]
                                                                  thenFetchManyRelationship);

        IEnumerable<T> ToFuture<T>(IQueryable<T> queryable);
    }
}