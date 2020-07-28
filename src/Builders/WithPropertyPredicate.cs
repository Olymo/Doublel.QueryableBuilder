using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.ReflexionExtensions;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Doublel.QueryableBuilder.Builders
{
    internal class WithPropertyPredicate<T> : PropertyPredicate<T> where T : class
    {
        private readonly WithQueryPropertyAttribute _attribute;

        public WithPropertyPredicate(object queryObject, WithQueryPropertyAttribute attribute) : base(queryObject) => _attribute = attribute;

        internal override IQueryable<T> BuildPredicate(IQueryable<T> query, PropertyInfo property)
        {
            if (!typeof(T).PropertyCanBeAccessed(_attribute.PropertyToCompareWith))
            {
                throw new InvalidQueryPropertyException(_attribute.PropertyToCompareWith, typeof(T));
            }

            var propertyValue = property.GetValue(QueryObject);

            if (propertyValue != null)
            {
                if (property.PropertyType.Name != typeof(bool?).Name)
                {
                    throw new InvalidQueryPropertyException($"MustHaveOne query can only be defined on nullable bool property. Property: {_attribute.PropertyToCompareWith}", typeof(T));
                }

                var booleanPropertyValue = (bool)propertyValue;

                var @operator = booleanPropertyValue ? "!=" : "==";

                query = query.Where(Config, _attribute.PropertyToCompareWith + $" {@operator} null");
            }

            return query;
        }
    }
}
