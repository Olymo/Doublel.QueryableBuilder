using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.DynamicQueryBuilder.Extensions;
using Doublel.DynamicQueryBuilder.Search;
using Doublel.QueryableBuilder.Builders;
using Doublel.QueryableBuilder.Extensions;
using Doublel.ReflexionExtensions;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Doublel.DynamicQueryBuilder
{
    internal class QueryBuilder
    {
        private readonly object _filterSource;

        internal QueryBuilder(object filterSource)
        {
            _filterSource = filterSource;
        }

        internal virtual IQueryable<T> BuildQuery<T>(IQueryable<T> source)
            where T : class
        {
            if (_filterSource == null)
            {
                return source;
            }

            var whereBuilder = new WhereBuilder<T>(source, _filterSource);
            source = whereBuilder.Build();

            if (_filterSource is ISortableSearch search && search.Sorts.Any())
            {
                var orderByBuilder = new OrderByBuilder<T>(source);
                source = orderByBuilder.Build(search);
            }

            return source;
        }

        internal virtual object BuildDynamicQuery<T>(IQueryable<T> source) 
            where T : class
        {
            var query = BuildQuery<T>(source);

            if(_filterSource is IPagedSearch search && search.Paginate)
            {
               return query.AsPagedResponse<T>(search.PerPage, search.Page);
            }

            return query.ToList();
        }

        internal virtual object BuildDynamicQuery<TIn, TOut>(IQueryable<TIn> source, Expression<Func<TIn, TOut>> transform)
            where TIn : class
            where TOut : class
        {
            var query = BuildQuery(source);

            if (_filterSource is IPagedSearch search && search.Paginate)
            {
                return query.AsPagedResponse(transform, search.PerPage, search.Page);
            }

            return query.Select(transform).ToList();
        }
    }
}
