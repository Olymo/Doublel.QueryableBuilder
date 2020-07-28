using Doublel.DynamicQueryBuilder;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Doublel.QueryableBuilder.Builders
{
    internal abstract class PropertyPredicate<T> where T : class
    {
        protected PropertyPredicate(object queryObject)
        {
            QueryObject = queryObject;
            Config = new ParsingConfig
            {
                EvaluateGroupByAtDatabase = true
            };
        }

        protected ParsingConfig Config { get; }

        protected object QueryObject { get; }
        internal abstract IQueryable<T> BuildPredicate(IQueryable<T> query, PropertyInfo property);
        internal string MakePredicate(ComparisonOperator @operator)
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
