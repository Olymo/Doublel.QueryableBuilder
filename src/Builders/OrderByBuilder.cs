using Doublel.DynamicQueryBuilder.Exceptions;
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
                if (!typeof(T).PropertyCanBeAccessed(sortAttribute.SortPropertyName.Replace("_", ".").FirstCharToUpper()))
                {
                    throw new SortPropertyNotFoundException(sortAttribute.SortPropertyName.FirstCharToUpper());
                }

                orderByClause += sortAttribute.SortPropertyName.Replace("_", ".") + " " + sortAttribute.SortDirection.ToString().ToLower() + ",";
            }

            orderByClause = orderByClause.Trim(',');

            return string.IsNullOrEmpty(orderByClause) ? BuildDefaultOrderBy() : _query.OrderBy(orderByClause);
        }

        private IOrderedQueryable<T> BuildDefaultOrderBy()
        {
            var propertyToOrderWith = typeof(T).GetProperties().First().Name;
            return _query.OrderBy(propertyToOrderWith + " asc");
        }
    }
}
