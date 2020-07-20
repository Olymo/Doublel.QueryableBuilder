using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Sort
{
    public class QuerySort
    {
        public QuerySort(string propertyName, QuerySortDirection sortDirection)
        {
            SortPropertyName = propertyName;

            SortDirection = sortDirection;
        }

        public string SortPropertyName { get; }
        public QuerySortDirection SortDirection { get; }
    }
}
