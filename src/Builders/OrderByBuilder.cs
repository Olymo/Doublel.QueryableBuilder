﻿using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.DynamicQueryBuilder.Extensions;
using Doublel.DynamicQueryBuilder.Search;
using System.Linq.Dynamic.Core;
using Doublel.ReflexionExtensions;
using System.Linq;

namespace Doublel.QueryableBuilder.Builders
{
    public class OrderByBuilder<T> where T : class
    {
        private IQueryable<T> _query;

        public OrderByBuilder(IQueryable<T> query) => _query = query;

        internal IQueryable<T> Build(ISortableSearch search)
        {
            var orderByClause = "";

            foreach (var sortAttribute in search.Sorts)
            {
                if (!typeof(T).PropertyCanBeAccessed(sortAttribute.SortPropertyName.FirstCharToUpper()))
                {
                    throw new SortPropertyNotFoundException(sortAttribute.SortPropertyName.FirstCharToUpper());
                }

                orderByClause += sortAttribute.SortPropertyName + " " + sortAttribute.SortDirection.ToString().ToLower() + ",";
            }

            orderByClause = orderByClause.Trim(',');

            return _query.OrderBy(orderByClause);
        }
    }
}
