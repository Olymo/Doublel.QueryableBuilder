using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.QueryableBuilder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.QueryableBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class WithAnyPropertyAttribute : Attribute
    {
        public ComparisonOperator Operator { get; }
        private readonly string _propertyToCompareWith;

        public WithAnyPropertyAttribute(string propertyToCompareWith = null, ComparisonOperator @operator = ComparisonOperator.Equals)
        {
            Operator = @operator;
            if(!propertyToCompareWith.Contains("."))
            {
                throw new InvalidOperationException("WithAnyPropertyAttribute can only be used on navigation properties.");
            }

            _propertyToCompareWith = propertyToCompareWith;
        }

        public string CollectionProprtyPath => _propertyToCompareWith.Substring(0, _propertyToCompareWith.LastIndexOf("."));
        public string PropertyNameToCompareWith => _propertyToCompareWith.Substring(_propertyToCompareWith.LastIndexOf(".") + 1);
    }
}
