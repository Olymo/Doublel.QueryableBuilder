using System;
using System.Collections.Generic;
using System.Text;

namespace Doublel.QueryableBuilder.Test.InMemoryTests
{
    public class AllCombinedTest
    {
        public void AllCombinationsWork()
        {

        }

        private class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int NumberOfTimesHasBeenBought { get; set; }

            public Category Category { get; set; }
            public IEnumerable<OrderLine> OrderLines { get; set; }
        }

        private class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class Attribute
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class ProductAttribute
        {
            public Product Product { get; set; }
            public Attribute Attribute { get; set; }
            public string Value { get; set; }
        }

        private class OrderLine
        {
            public int Id { get; set; }
            public decimal Price { get; set; }
            public string Name { get; set; }
            public Product Product { get; set; }
            public Order Order { get; set; }
        }

        private class Order
        {
            public int Id { get; set; }
            public DateTime OrderedAt { get; set; }
            public IEnumerable<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
        }
    }
}
