using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Search;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Doublel.QueryableBuilder.Test.Ef6Tests
{
    public class QueryPropertyTests
    {
        [Fact]
        public void OrderSearchByCustomerId()
        {
            var orders = Context.Orders.BuildQuery(new OrderSearch { });
            orders.Count().Should().Be(830);

            orders = Context.Orders.BuildQuery(new OrderSearch { CustomerID = "LILAS" });
            orders.Count().Should().Be(14);
        }

        [Fact]
        public void NonPagedResponseWorks()
        {
            var orders = Context.Orders.BuildDynamicQuery(new OrderSearch
            {
                CustomerID = "LILAS",
                PerPage = 5,
                Page = 1
            });

            orders.Should().BeOfType<List<Order>>();
            var orderlist = orders as List<Order>;
            orderlist.Should().HaveCount(14);
        }

        [Fact]
        public void PagedResponseWorks()
        {
            var orders = Context.Orders.BuildDynamicQuery(new OrderSearch
            {
                CustomerID = "LILAS",
                PerPage = 5,
                Page = 1,
                Paginate = true
            });

            orders.Should().BeOfType<PagedResponse<Order>>();
            var orderlist = orders as PagedResponse<Order>;
            orderlist.TotalCount.Should().Be(14);
            orderlist.Items.Should().HaveCount(5);
        }

        [Fact]
        public void PagedResponseWorksWithSortBy()
        {
            var orders = Context.Orders.BuildDynamicQuery(new OrderSearch
            {
                CustomerID = "LILAS",
                PerPage = 5,
                Page = 1,
                Paginate = true,
                SortBy = "EmployeeID.ASC"
            });

            orders.Should().BeOfType<PagedResponse<Order>>();
            var orderlist = orders as PagedResponse<Order>;
            orderlist.TotalCount.Should().Be(14);
            orderlist.Items.Should().HaveCount(5);
        }

        public Model1 Context => new Model1();

        public class OrderSearch : SortablePagedSearch
        {
            [QueryProperty]
            public string CustomerID { get; set; }
        }
    }
}
