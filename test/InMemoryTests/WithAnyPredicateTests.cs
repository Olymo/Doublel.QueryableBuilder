using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Exceptions;
using Doublel.QueryableBuilder.Attributes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Doublel.QueryableBuilder.Test.InMemoryTests
{
    public class WithAnyPredicateTests
    {
        [Fact]
        public void ThrowsException_WhenCollectionDoesntExistOnResultingType()
        {
            Action a = () => Products.BuildQuery(new WProductQuery { AttrName = "a" });
            a.Should().ThrowExactly<InvalidQueryPropertyException>();
        }

        [Fact]
        public void ThrowsException_WhenPropertyDoesntExistOnResultingCollection()
        {
            Action a = () => Products.BuildQuery(new WProductQuery1 { AttrName = "a" });
            a.Should().ThrowExactly<InvalidQueryPropertyException>();
        }


        [Fact]
        public void Works()
        {
            var result = Products.BuildQuery(new WProductQuery2 { AttrName = "Attr1" });
            result.Should().HaveCount(1);

            result = Products.BuildQuery(new WProductQuery2 { Attr = "CAt", WithCategory = true });
            result.Should().HaveCount(1);
        }

        private IQueryable<TestProduct> Products => new List<TestProduct> 
        { 
            new TestProduct { Id = 1, Name = "Prod 1", Attributes = new List<TestAttribute> { new TestAttribute { Id = 1, Name = "Attr1" }}},
            new TestProduct { Id = 2, Name = "Prod 2", Attributes = new List<TestAttribute> { new TestAttribute { Id = 3, Name = "Atribut 3" }}},
            new TestProduct { Id = 2, Name = "Prod 2", Attributes = new List<TestAttribute> { new TestAttribute { Id = 3, Name = "Atribut 3" }}, 
            Category = new WCategory 
            { 
                Attributes = new List<TestAttribute> { new TestAttribute { Id = 4, Name = "CAttribute" }, new TestAttribute { Name = "Cattribute2" } }
            }}
        }.AsQueryable();
        
    }

    public class WProductQuery
    {
        [WithAnyProperty("Attribute.Name")]
        public string AttrName { get; set; }
    }

    public class WProductQuery1
    {
        [WithAnyProperty("Attributes.Name1")]
        public string AttrName { get; set; }
    }

    public class WProductQuery2
    {
        [WithAnyProperty("Attributes.Name")]
        public string AttrName { get; set; }
        [WithQueryProperty]
        public bool? WithCategory { get; set; }
        [WithAnyProperty("Category.Attributes.Name", ComparisonOperator.Contains)]
        public string Attr { get; set; }
    }

    public class TestProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TestAttribute> Attributes  { get; set; }
        public WCategory Category { get; set; }
    }

    public class WCategory
    {
        public string Name { get; set; }
        public IEnumerable<TestAttribute> Attributes { get; set; }
    }

    public class TestAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }



}
