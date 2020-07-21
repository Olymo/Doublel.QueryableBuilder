using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.DynamicQueryBuilder.Extensions;
using Doublel.DynamicQueryBuilder.Search;
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
        private readonly ParsingConfig _config;

        internal QueryBuilder(object filterSource)
        {
            _filterSource = filterSource;
            _config = new ParsingConfig
            {
                EvaluateGroupByAtDatabase = true
            };
        }

        internal virtual IQueryable<T> BuildQuery<T>(IQueryable<T> source)
            where T : class
        {
            if (_filterSource == null)
            {
                return source;
            }

            source = BuildWhereClause(source);

            if (_filterSource is ISortableSearch search && search.Sorts.Any())
            {
                source = BuildOrderBy(source, search);
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

        private IQueryable<T> BuildOrderBy<T>(IQueryable<T> source, ISortableSearch search) where T : class
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

            source = source.OrderBy(orderByClause);

            return source;
        }

        private IQueryable<T> BuildWhereClause<T>(IQueryable<T> collection) where T : class
        {
            foreach (var property in _filterSource.GetType().GetProperties())
            {
                if (property.HasAttribute<QueryProperty>() && property.HasAttribute<QueryProperties>())
                {
                    throw new InvalidOperationException("Property can be decorated using only one of QueryProperty attributes at once.");
                }

                if (property.HasAttribute<QueryProperty>() || property.HasAttribute<NavigationQueryPropertyAttribute>())
                {
                    collection = AppendWhereSingleProperty(collection, property);
                }

                if (property.HasAttribute<QueryProperties>())
                {
                    collection = AppendWhereMorePropertiesDividedByOrOperator(collection, property);

                }
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

        private IQueryable<T> AppendWhereSingleProperty<T>(IQueryable<T> collection, PropertyInfo property) where T : class
        {
            var queryProperty = property.GetAttribute<QueryProperty>();

            if (queryProperty != null)
            {
                return FilterByQueryProperty(collection, property, queryProperty);
            }

            var mustHaveOneProperty = property.GetAttribute<NavigationQueryPropertyAttribute>();

            return FilterByMustHaveOneProperty(collection, property, mustHaveOneProperty);
        }

        private IQueryable<T> FilterByMustHaveOneProperty<T>(IQueryable<T> collection, PropertyInfo property, NavigationQueryPropertyAttribute attribute) where T : class
        {
            if (!typeof(T).PropertyCanBeAccessed(attribute.PropertyToCompareWith) && !attribute.IsNavigationProperty)
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
