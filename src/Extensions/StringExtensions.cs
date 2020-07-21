using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input must be non nullable and non empty.");
            }

            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
