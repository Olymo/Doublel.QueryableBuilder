using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.ReflexionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Doublel.QueryableBuilder.Builders
{
    internal class QueryPropertiesPredicate<T> : PropertyPredicate<T> where T : class
    {
        private readonly QueryProperties _attribute;

        internal QueryPropertiesPredicate(object queryObject, QueryProperties attribute) : base(queryObject) => _attribute = attribute;


        internal override IQueryable<T> BuildPredicate(IQueryable<T> query, PropertyInfo property)
        {
            var orPartOfTheQuery = "";

            foreach (var propertyName in _attribute.PropertiesToCompareWith)
            {
                if (!typeof(T).PropertyCanBeAccessed(propertyName))
                {
                    throw new InvalidQueryPropertyException(propertyName, typeof(T));
                }

                if (orPartOfTheQuery == "")
                {
                    orPartOfTheQuery = propertyName + MakePredicate(_attribute.Operator);
                }
                else
                {
                    orPartOfTheQuery += $" || {propertyName}{MakePredicate(_attribute.Operator)}";
                }
            }

            var propertyValue = property.GetValue(QueryObject);

            if (propertyValue != null)
            {
                query = query.Where(Config, orPartOfTheQuery, propertyValue);
            }

            return query;
        }
    }
}
