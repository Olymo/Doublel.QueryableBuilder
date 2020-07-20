using System;
using System.Runtime.CompilerServices;

namespace Doublel.DynamicQueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QueryProperty : Attribute
    {
        public ComparisonOperator Operator { get; }
        public string PropertyToCompareWith { get; }
        public bool IsNavigationProperty => PropertyToCompareWith.Contains(".");

        public QueryProperty(ComparisonOperator @operator = ComparisonOperator.Equals, string propertyToCompareWith = null, [CallerMemberName] string propertyName = null)
        {
            Operator = @operator;
            PropertyToCompareWith = propertyToCompareWith ?? propertyName;
        }
    }

    
}
