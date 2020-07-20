using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Attributes
{
    public class QueryProperties : Attribute
    {
        public IEnumerable<string> PropertiesToCompareWith { get; }
        public ComparisonOperator Operator { get; }
        public QueryProperties(ComparisonOperator @operator, params string[] properties)
        {
            PropertiesToCompareWith = properties;
            Operator = @operator;
        }
    }
}
