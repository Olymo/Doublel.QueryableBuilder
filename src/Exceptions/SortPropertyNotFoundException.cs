using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Exceptions
{
    public class SortPropertyNotFoundException : Exception
    {
        public SortPropertyNotFoundException(string propertyName)
            : base($"Sort property {propertyName} doesnt exist on resulting type.")
        {

        }
    }
}
