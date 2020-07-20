using Doublel.DynamicQueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Doublel.QueryableBuilder.Extensions
{
    public static class QueryableExtensions
    {
        public static PagedResponse<T> AsPagedResponse<T>(this IQueryable<T> source, int perPage = 10, int currentPage = 1)
            where T : class
        {
            var itemsToSkip = perPage * (currentPage - 1);

            return new PagedResponse<T>
            {
                TotalCount = source.Count(),
                CurrentPage = currentPage,
                ItemsPerPage = perPage,
                Items = source.Skip(itemsToSkip).Take(perPage).ToList()
            };
        }

        public static PagedResponse<TOut> AsPagedResponse<TIn, TOut>(this IQueryable<TIn> source, Expression<Func<TIn, TOut>> transform, int perPage = 10, int currentPage = 1)
            where TIn : class
            where TOut : class
        {
            var itemsToSkip = perPage * (currentPage - 1);

            return new PagedResponse<TOut>
            {
                TotalCount = source.Count(),
                CurrentPage = currentPage,
                ItemsPerPage = perPage,
                Items = source.Skip(itemsToSkip).Take(perPage).Select(transform).ToList()
            };
        }
    }
}
