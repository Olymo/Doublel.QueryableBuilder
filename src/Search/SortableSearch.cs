using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.DynamicQueryBuilder.Sort;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Search
{
    public class SortableSearch : ISortableSearch
    {
        /// <summary>
        /// Sort by expected by actual clients. Expected format - PropertyName.Direction(asc|desc) separated by ,
        /// Example - FirstName.DESC,LastName.ASC
        /// </summary>
        public string SortBy { get; set; }
        
        public IEnumerable<QuerySort> Sorts
        {
            get
            {
                var sorts = new List<QuerySort>();

                if (string.IsNullOrEmpty(SortBy))
                {
                    return sorts;
                }
                var sortStringRepresentation = SortBy.Split(',');
                foreach (var sort in sortStringRepresentation)
                {
                    var actualSortItem = sort.Split('.');

                    try
                    {
                        if (actualSortItem[1].ToLower() != "asc" && actualSortItem[1].ToLower() != "desc")
                        {
                            throw new InvalidSortDirectionException("Invalid sort direction for property ");
                        }

                        var sortDirection = actualSortItem[1].ToLower() == "asc" ? QuerySortDirection.ASC : QuerySortDirection.DESC;

                        sorts.Add(new QuerySort(actualSortItem[0], sortDirection));
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidSortPropertyException();
                    }
                }

                return sorts;
            }
        }
    }

    public class SortablePagedSearch : SortableSearch, IPagedSearch
    {
        public int PerPage { get; set; }

        public int Page { get; set;}

        public bool Paginate { get; set; }
    }
}
