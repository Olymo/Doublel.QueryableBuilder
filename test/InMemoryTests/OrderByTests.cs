using Doublel.DynamicQueryBuilder.Search;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Doublel.QueryableBuilder.Test.InMemoryTests
{
    public class OrderByTests
    {
        [Fact]
        public void OrderingWorksInIsolation()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id  = 1,
                    Name = "Product 1",
                    Price = 100,
                    Category = new Category
                    {
                        Name = "F"
                    }
                },
                new Product
                {
                    Id  = 1,
                    Name = "Product 2",
                    Price = 30,
                    Category = new Category {
                        Name = "A"
                    }
                },
                new Product
                {
                    Id  = 1,
                    Name = "Product 3",
                    Price = 30
                },
                new Product
                {
                    Id  = 1,
                    Name = "Product 1",
                    Price = 500
                },new Product
                {
                    Id  = 1,
                    Name = "Product 1",
                    Price = 300
                }
            };

            var queryObject = new SortableSearch
            {
                SortBy = "Price.DESC,Name.ASC"
            };

            var result = products.AsQueryable().BuildQuery(queryObject).ToList();
            result.First().Name.Should().Be("Product 1");
            result.First().Price.Should().Be(500);
            result.Last().Name.Should().Be("Product 3");
            result.Last().Price.Should().Be(30);
        }

        [Fact]
        public void OrderingWorksInNestedObjects()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id  = 1,
                    Name = "Product 1",
                    Price = 100,
                    Category = new Category
                    {
                        Name = "F"
                    }
                },
                new Product
                {
                    Id  = 1,
                    Name = "Product 2",
                    Price = 30,
                    Category = new Category {
                        Name = "A"
                    }
                },
                new Product
                {
                    Id  = 1,
                    Name = "Product 3",
                    Price = 30,
                    Category = new Category {}
                },
                new Product
                {
                    Id  = 1,
                    Name = "Product 1",
                    Price = 500,
                    Category = new Category {}
                },new Product
                {
                    Id  = 1,
                    Name = "Product 1",
                    Price = 300,
                    Category = new Category {}
                }
            };

            var queryObject = new SortableSearch
            {
                SortBy = "Category_Name.ASC"
            };

            var result = products.AsQueryable().BuildQuery(queryObject).ToList();
            result.First().Name.Should().Be("Product 2");
        }
    }
 
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
    }

    public class Category
    {
        public string Name { get; set; } = "W";
    }
}
