using Doublel.DynamicQueryBuilder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Doublel.QueryableBuilder
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Builds query based on buildObject. Build object can include wheres & orders.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="buildObject"></param>
        /// <returns></returns>
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> query, object buildObject)
            where T : class
        {
            var builder = new QueryBuilder(buildObject);
            return builder.BuildQuery(query);
        }

        /// <summary>
        /// Builds query on top of DbSet based on query object. Build object can include wheres & orders.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="buildObject"></param>
        /// <returns></returns>
        public static IQueryable<T> BuildQuery<T>(this DbSet<T> dbSet, object buildObject)
            where T : class => dbSet.AsQueryable().BuildQuery(buildObject);

        public static IQueryable<TOut> BuildQuery<T, TOut>(this IQueryable<T> query, object buildObject, Expression<Func<T, TOut>> project)
            where T : class
            where TOut : class => query.BuildQuery(buildObject).Select(project);


        public static object BuildDynamicQuery<T>(this IQueryable<T> query, object buildObject)
            where T : class
        {
            var builder = new QueryBuilder(buildObject);

            return builder.BuildDynamicQuery(query);
        }


        public static object BuildDynamicQuery<T>(this DbSet<T> dbSet, object buildObject)
            where T : class => dbSet.AsQueryable().BuildDynamicQuery(buildObject);

        public static object BuildDynamicQuery<T, TOut>(this IQueryable<T> query, object buildObject, Expression<Func<T, TOut>> project)
             where T : class
            where TOut : class
        {
            var queryBuilder = new QueryBuilder(buildObject);

            return queryBuilder.BuildDynamicQuery(query);
        }

    }
}
