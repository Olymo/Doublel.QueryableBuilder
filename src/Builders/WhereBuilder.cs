using Doublel.DynamicQueryBuilder;
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
    internal class WhereBuilder<T> where T : class
    {
        private IQueryable<T> _query;
        private readonly ParsingConfig _config;
        private readonly object _filterSource;

        internal WhereBuilder(IQueryable<T> query, object filterSource)
        {
            _query = query;
            _config = new ParsingConfig
            {
                EvaluateGroupByAtDatabase = true
            };
            _filterSource = filterSource;
        }

        internal IQueryable<T> Build()
        {
            foreach (var property in _filterSource.GetType().GetProperties())
            {
                if (property.HasAttribute<QueryProperty>() && property.HasAttribute<QueryProperties>())
                {
                    throw new InvalidOperationException("Property can be decorated using only one of QueryProperty attributes at once.");
                }

                if (property.HasAttribute<QueryProperty>() || property.HasAttribute<WithQueryPropertyAttribute>())
                {
                    _query = AppendWhereSingleProperty(_query, property);
                }

                if (property.HasAttribute<QueryProperties>())
                {
                    _query = AppendWhereMorePropertiesDividedByOrOperator(_query, property);

                }
            }

            return _query;
        }

        private IQueryable<T> AppendWhereSingleProperty<T>(IQueryable<T> collection, PropertyInfo property) where T : class
        {
            var queryProperty = property.GetAttribute<QueryProperty>();

            if (queryProperty != null)
            {
                return FilterByQueryProperty(collection, property, queryProperty);
            }

            var mustHaveOneProperty = property.GetAttribute<WithQueryPropertyAttribute>();

            return FilterByWith(collection, property, mustHaveOneProperty);
        }

        private IQueryable<T> FilterByQueryProperty<T>(IQueryable<T> collection, PropertyInfo property, QueryProperty attribute) where T : class
        {
            if (!typeof(T).PropertyCanBeAccessed(attribute.PropertyToCompareWith) && !attribute.IsNavigationProperty)
            {
                throw new InvalidQueryPropertyException(attribute.PropertyToCompareWith, typeof(T));
            }

            var propertyValue = property.GetValue(_filterSource);

            if (propertyValue != null)
            {
                collection = collection.Where(_config, attribute.PropertyToCompareWith + $"{MakePredicate(attribute.Operator)}", propertyValue);
            }

            return collection;
        }

        private IQueryable<T> FilterByWith<T>(IQueryable<T> collection, PropertyInfo property, WithQueryPropertyAttribute attribute) where T : class
        {
            if (!typeof(T).PropertyCanBeAccessed(attribute.PropertyToCompareWith))
            {
                throw new InvalidQueryPropertyException(attribute.PropertyToCompareWith, typeof(T));
            }

            var propertyValue = property.GetValue(_filterSource);

            if (propertyValue != null)
            {
                if (property.PropertyType.Name != typeof(bool?).Name)
                {
                    throw new InvalidQueryPropertyException($"MustHaveOne query can only be defined on nullable bool property. Property: {attribute.PropertyToCompareWith}", typeof(T));
                }

                var booleanPropertyValue = (bool)propertyValue;

                var @operator = booleanPropertyValue ? "!=" : "==";

                collection = collection.Where(_config, attribute.PropertyToCompareWith + $" {@operator} null");
            }

            return collection;
        }

        private IQueryable<T> AppendWhereMorePropertiesDividedByOrOperator<T>(IQueryable<T> collection, PropertyInfo property) where T : class
        {
            var attribute = property.GetAttribute<QueryProperties>();

            var orPartOfTheQuery = "";

            foreach (var propertyName in attribute.PropertiesToCompareWith)
            {
                if (!typeof(T).PropertyCanBeAccessed(propertyName))
                {
                    throw new InvalidQueryPropertyException(propertyName, typeof(T));
                }

                if (orPartOfTheQuery == "")
                {
                    orPartOfTheQuery = propertyName + MakePredicate(attribute.Operator);
                }
                else
                {
                    orPartOfTheQuery += $" || {propertyName}{MakePredicate(attribute.Operator)}";
                }
            }

            var propertyValue = property.GetValue(_filterSource);

            if (propertyValue != null)
            {
                collection = collection.Where(_config, orPartOfTheQuery, propertyValue);
            }

            return collection;
        }

        private string MakePredicate(ComparisonOperator @operator)
        {
            switch (@operator)
            {
                case ComparisonOperator.MoreThan:
                    return $" >  @0";
                case ComparisonOperator.NotEqual:
                    return $" !=  @0";
                case ComparisonOperator.MoreThanOrEqualsTo:
                    return $" >=  @0";
                case ComparisonOperator.LessThan:
                    return $" <  @0";
                case ComparisonOperator.LessThanOrEqualsTo:
                    return $" <=  @0";
                case ComparisonOperator.Contains:
                    return $".ToLower().Contains(@0.ToLower())";
                case ComparisonOperator.StartsWith:
                    return $".ToLower().StartsWith(@0.ToLower())";
                case ComparisonOperator.EndsWith:
                    return $".ToLower().EndsWith(@0.ToLower())";
                default:
                    break;
            }

            return "==  @0";
        }
    }
}
