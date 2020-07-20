using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Exceptions
{
    public class InvalidQueryPropertyException : Exception
    {
        public InvalidQueryPropertyException(string propertyName, Type t) :
            base($"Property of name {propertyName} doesn't exist on resulting type: {t.FullName}.")
        {

        }
    }
}
