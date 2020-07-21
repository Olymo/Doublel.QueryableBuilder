using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Doublel.DynamicQueryBuilder.Attributes
{
    public class WithQueryPropertyAttribute : Attribute
    {
        public string PropertyToCompareWith { get; }
        public bool IsNavigationProperty => PropertyToCompareWith.Contains(".");
        public WithQueryPropertyAttribute(string propertyToCompareWith = null, [CallerMemberName] string propertyName = null)
        {
            var name = propertyToCompareWith ?? propertyName;

            if (name.Contains("With"))
            {
                name = name.Split(new string[] { "With" }, StringSplitOptions.None)[1];
            }

            PropertyToCompareWith = name;
        }
    }
}
