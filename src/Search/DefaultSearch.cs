using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Search
{
    public class DefaultSearch : IPagedSearch
    {
        public int PerPage { get; set; } = 10;
        public int Page { get; set; } = 1;
        public bool Paginate { get; set; } = false;
    }
}
