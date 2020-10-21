using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Search;
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
        /// <param name="queryObject"></param>
        /// <returns></returns>
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> query, object queryObject)
            where T : class
        {
            var builder = new QueryBuilder(queryObject);
            return builder.BuildQuery(query);
        }

        public static IQueryable<TOut> BuildQuery<T, TOut>(this IQueryable<T> query, object queryObject, Expression<Func<T, TOut>> project)
            where T : class
            where TOut : class => query.BuildQuery(queryObject).Select(project);


        public static object BuildDynamicQuery<T>(this IQueryable<T> query, object queryObject)
            where T : class
        {
            var builder = new QueryBuilder(queryObject);

            return builder.BuildDynamicQuery(query);
        }

        public static object BuildDynamicQuery<T, TOut>(this IQueryable<T> query, object queryObject, Expression<Func<T, TOut>> project)
             where T : class
            where TOut : class
        {
            var queryBuilder = new QueryBuilder(queryObject);

            if(queryObject is IPagedSearch search && search.Paginate)
            {
                var paged = queryBuilder.BuildDynamicQuery(query) as PagedResponse<T>;

                return new PagedResponse<TOut>
                {
                    CurrentPage = paged.CurrentPage,
                    ItemsPerPage = paged.ItemsPerPage,
                    Items = paged.Items.Select(project.Compile()),
                    TotalCount = paged.TotalCount
                };
            }
            else
            {
                return query.BuildQuery(queryObject, project).ToList();
            }
        }

    }
}
