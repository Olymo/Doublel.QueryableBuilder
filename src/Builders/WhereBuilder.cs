using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.QueryableBuilder.Attributes;
using Doublel.ReflexionExtensions;
using System;
using System.Linq;
using System.Reflection;

namespace Doublel.QueryableBuilder.Builders
{
    internal class WhereBuilder<T> where T : class
    {
        private IQueryable<T> _query;
        private readonly object _queryObject;

        internal WhereBuilder(IQueryable<T> query, object queryObject)
        {
            _query = query;
            _queryObject = queryObject;
        }

        internal IQueryable<T> Build()
        {
            foreach (var property in _queryObject.GetType().GetProperties())
            {

                var predicateBuilder = GetPredicate(property);

                //This situation happens when there is a property on query object not decorated by one of our decorators, we're just skipping that property
                if(predicateBuilder == null)
                {
                    continue;
                }

                _query = predicateBuilder.BuildPredicate(_query, property);
            }

            return _query;
        }

        private PropertyPredicate<T> GetSingleWherePredicate(PropertyInfo property)
        {
            var queryProperty = property.GetAttribute<QueryProperty>();

            if (queryProperty != null)
            {
                return new QueryPropertyPredicate<T>(_queryObject, queryProperty);
            }

            var mustHaveOneProperty = property.GetAttribute<WithQueryPropertyAttribute>();

            return new WithPropertyPredicate<T>(_queryObject, mustHaveOneProperty);
        }

        private PropertyPredicate<T> GetPredicate(PropertyInfo property)
        {
            if (property.HasAttribute<QueryProperty>() && property.HasAttribute<QueryProperties>())
            {
                throw new InvalidOperationException("Property can be decorated using only one of QueryProperty attributes at once.");
            }

            if (property.HasAttribute<QueryProperty>() || property.HasAttribute<WithQueryPropertyAttribute>())
            {
                return GetSingleWherePredicate(property);
            }

            if (property.HasAttribute<QueryProperties>())
            {
                var attribute = property.GetAttribute<QueryProperties>();

                return new QueryPropertiesPredicate<T>(_queryObject, attribute);
            }

            if (property.HasAttribute<WithAnyPropertyAttribute>())
            {
                var attribute = property.GetAttribute<WithAnyPropertyAttribute>();

                return new WithAnyPredicate<T>(_queryObject, attribute);
            }

            return null;
        }
    }
}
