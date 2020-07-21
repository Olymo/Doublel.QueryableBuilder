using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Search;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Doublel.QueryableBuilder.Test.InMemoryTests
{
    public class NonNestedSinglePropertyTests
    {
        [Fact]
        public void ReturnsEntireQueryable_WhenBuildObjectIsNUll()
        {
            UserQuery.BuildQuery(null).Should().HaveCount(3);
        }

        [Fact]
        public void WithPropertyWorksInIsolation()
        {
            var result = UserQuery.BuildQuery(new TestUserSearch { WithMarriedAt = true }).ToList();
            result.Should().HaveCount(1);
            result.First().FirstName.Should().Be("Tim");
            result.First().Username.Should().Be("tim1");

            result = UserQuery.BuildQuery(new TestUserSearch()).ToList();
            result.Should().HaveCount(3);
            result.First().FirstName.Should().Be("John");
            result.ElementAt(1).FirstName.Should().Be("Mark");
            result.ElementAt(2).FirstName.Should().Be("Tim");

            result = UserQuery.BuildQuery(new TestUserSearch { WithMarriedAt = false }).ToList();
            result.Should().HaveCount(2);
            result.First().FirstName.Should().Be("John");
            result.ElementAt(1).FirstName.Should().Be("Mark");
        }

        [Fact]
        public void QueryPropertyWorksInIsolation()
        {
            var result = UserQuery.BuildQuery(new TestUserSearch { Age = 20 });
            result.Should().HaveCount(2);
            result.All(x => x.Age == 20).Should().BeTrue();
            result.Any(x => x.Age != 20).Should().BeFalse();
        }

        [Fact]
        public void QueryPropertiesWorksInIsolation()
        {
            var result = UserQuery.BuildQuery(new TestUserSearch { Keyword = "m" });
            result.Should().HaveCount(2);
            result.All(x => x.Age == 20).Should().BeTrue();
        }
        
        [Fact]
        public void QueryPropertyWithProjectionWorksInIsolation()
        {
            var result = UserQuery.BuildQuery(new TestUserSearch { Age = 10 }, x => new TestuserDto 
            { 
                Age = x.Age,
                Name = x.FirstName
            });

            result.Should().HaveCount(1);
            result.All(x => x.Age == 10).Should().BeTrue();
            result.Any(x => x.Age != 10).Should().BeFalse();
            result.First().Name.Should().Be("John");
        }

        private IQueryable<TestUser> UserQuery
        {
            get
            {
                return new List<TestUser>
                {
                    new TestUser { Id  = 1, Age = 10, FirstName = "John", Username = "john1", MarriedAt = null },
                    new TestUser { Id  = 2, Age = 20, FirstName = "Mark", Username = "mark1", MarriedAt = null },
                    new TestUser { Id  = 3, Age = 20, FirstName = "Tim", Username = "tim1", MarriedAt = new DateTime(2012 ,12 ,12) }
                }.AsQueryable();
            }
        }
    }

    public class TestUserSearch : DefaultSearch
    {
        [WithQueryProperty]
        public bool? WithMarriedAt { get; set; }
        [QueryProperty]
        public int? Age { get; set; }
        [QueryProperties(ComparisonOperator.Contains, "FirstName", "Username")]
        public string Keyword { get; set; }
    }

    public class TestuserDto
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }

    public class TestUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public DateTime? MarriedAt { get; set; }
    }
}
