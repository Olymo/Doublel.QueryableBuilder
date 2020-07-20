using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Search
{
    public interface IPagedSearch : ISearch
    {
        int PerPage { get; }
        int Page { get; }
        bool Paginate { get; }
    }

    public interface ISearch
    {

    }
}
