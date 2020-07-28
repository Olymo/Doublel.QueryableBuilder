using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.QueryableBuilder.Attributes;
using Doublel.ReflexionExtensions;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Doublel.QueryableBuilder.Builders
{
    internal class WithAnyPredicate<T> : PropertyPredicate<T> where T : class
    {
        private readonly WithAnyPropertyAttribute _attribute;
        public WithAnyPredicate(object queryObject, WithAnyPropertyAttribute attribute) : base(queryObject)
        {
            _attribute = attribute;
        }

        internal override IQueryable<T> BuildPredicate(IQueryable<T> query, PropertyInfo property)
        {
            if (!typeof(T).PropertyCanBeAccessed(_attribute.CollectionProprtyPath))
            {
                throw new InvalidQueryPropertyException(_attribute.CollectionProprtyPath, typeof(T));
            }

            var navigationParts = _attribute.CollectionProprtyPath.Split('.').AsEnumerable();

            var collectionPropetyType = typeof(T).GetProperty(navigationParts.First()).PropertyType;

            if (navigationParts.Count() > 1)
            {
                navigationParts = navigationParts.Skip(1);

                foreach (var path in navigationParts)
                {
                    collectionPropetyType = collectionPropetyType.GetProperty(path).PropertyType;
                }
            }

            if(!(collectionPropetyType.IsGenericType && collectionPropetyType.InheritsFrom<IEnumerable>() && collectionPropetyType.GenericTypeArguments.Any()))
            {
                throw new InvalidQueryPropertyException($"WithAnyPredicate can only be used on top of Generic collections.");
            }

            var propertyOfInterest = collectionPropetyType.GenericTypeArguments[0].GetProperty(_attribute.PropertyNameToCompareWith);
            
            if(propertyOfInterest == null)
            {
                throw new InvalidQueryPropertyException(_attribute.PropertyNameToCompareWith, collectionPropetyType.GenericTypeArguments[0]);
            }

            var valueToCompareAgainst = property.GetValue(QueryObject);

            if(valueToCompareAgainst == null)
            {
                return query;
            }
            var dynamicLinqToExecute = $"{_attribute.CollectionProprtyPath}.Any(x => x.{_attribute.PropertyNameToCompareWith} != null && x.{_attribute.PropertyNameToCompareWith}{MakePredicate(_attribute.Operator)})";
            
            return query.Where(dynamicLinqToExecute, valueToCompareAgainst);
        }
    }
}
