using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.ReflexionExtensions;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Doublel.QueryableBuilder.Builders
{
    internal class QueryPropertyPredicate<T> : PropertyPredicate<T> where T : class
    {
        private readonly QueryProperty _attribute;

        public QueryPropertyPredicate(object queryObject, QueryProperty attribute) : base(queryObject) => _attribute = attribute;

        internal override IQueryable<T> BuildPredicate(IQueryable<T> query, PropertyInfo property)
        {
            if (!typeof(T).PropertyCanBeAccessed(_attribute.PropertyToCompareWith) && !_attribute.IsNavigationProperty)
            {
                throw new InvalidQueryPropertyException(_attribute.PropertyToCompareWith, typeof(T));
            }

            var propertyValue = property.GetValue(QueryObject);

            if (propertyValue != null)
            {
                query = query.Where(Config, _attribute.PropertyToCompareWith + $"{MakePredicate(_attribute.Operator)}", propertyValue);
            }

            return query;
        }
    }
    
}
