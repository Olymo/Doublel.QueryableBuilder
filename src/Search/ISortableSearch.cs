using Doublel.DynamicQueryBuilder.Sort;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Search
{
    public interface ISortableSearch : ISearch
    {
        internal IEnumerable<QuerySort> Sorts { get; }
    }
}
