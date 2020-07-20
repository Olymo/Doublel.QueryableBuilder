using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Exceptions
{
    public class InvalidSortDirectionException : Exception
    {

        public InvalidSortDirectionException(string propertyName)
            : base($"Invalid sort direction for property {propertyName}. Allowed options are ASC and DESC.")
        {

        }
    }
}
