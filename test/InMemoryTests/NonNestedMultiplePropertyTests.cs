using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doublel.QueryableBuilder.Test.InMemoryTests
{
    public class NonNestedMultiplePropertyTests
    {
        private IQueryable<TestUser> UserQuery
        {
            get
            {
                return new List<TestUser>
                {
                    new TestUser { Id  = 1, Age = 25, FirstName = "Denmla", Username = "den", MarriedAt = new DateTime(2020, 8, 20) },
                    new TestUser { Id  = 2, Age = 23, FirstName = "Johnny", Username = "john", MarriedAt = null },
                    new TestUser { Id  = 3, Age = 24, FirstName = "Mark", Username = "mark", MarriedAt = new DateTime(2021, 1, 1) },
                    new TestUser { Id  = 4, Age = 22, FirstName = "Matthew", Username = "matt", MarriedAt = null },
                    new TestUser { Id  = 5, Age = 24, FirstName = "Moma", Username = "mom", MarriedAt = null },
                    new TestUser { Id  = 6, Age = 25, FirstName = "Alexander", Username = "lex", MarriedAt = null },
                    new TestUser { Id  = 7, Age = 15, FirstName = "Andrew", Username = "andy", MarriedAt = new DateTime(2012 ,12 ,12) },
                }.AsQueryable();
            }
        }
    }
}
