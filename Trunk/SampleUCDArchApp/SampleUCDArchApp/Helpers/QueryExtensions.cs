using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using NHibernate.Linq;

namespace SampleUCDArchApp.Helpers
{
    /*
    public static class QueryExtensions
    {
        public static INHibernateQueryable<T> NhQueryable<T>(this IQueryable<T> queryable)
        {
            if (queryable is INHibernateQueryable<T> == false)
            {
                throw new ArgumentException("The IQueryable instance is not extendable, make suer it inherits from INhibernateQueryable<T>");
            }

            return (INHibernateQueryable<T>)queryable;
        }

        public static IQueryable<T> Cache<T>(this IQueryable<T> queryable)
        {
            queryable.NhQueryable().QueryOptions.SetCachable(true);

            return queryable;
        }

        public static IQueryable<T> Fetch<T>(this IQueryable<T> queryable, Expression<Func<T, object>> property)
        {
            var nhquery = queryable.NhQueryable();
            nhquery.QueryOptions.AddExpansion(ExpressionHelper.GetExpressionText(property));

            return nhquery;
        }
    }
     **/
}