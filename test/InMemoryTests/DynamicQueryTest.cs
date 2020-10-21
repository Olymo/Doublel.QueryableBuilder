using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Search;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Doublel.QueryableBuilder.Test.InMemoryTests
{
    public class DynamicQueryTest
    {
        [Fact]
        public void CheckBuildQuery()
        {
            var query = new ProductQuery
            {
                Name = "duc"
            };

            var result = Products.BuildQuery(query, x => new ProductDto 
            {
                Id = x.Id,
                Name = x.Name
            });

            result.Should().HaveCount(3);
        }

        [Fact]
        public void CheckBuildDinamicQueryWithoutPaginate()
        {
            var query = new ProductQueryPaginateSortable
            {
                Name = null,
                SortBy = "Id.DESC"
            };

            var result = Products.BuildDynamicQuery(query, x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name
            });

            result.Should().BeOfType<IQueryable<ProductDto>>();
            var productList = result as IQueryable<ProductDto>;
            productList.Should().HaveCount(7);
        }

        [Fact]
        public void CheckBuildDinamicQueryWithPaginate()
        {
            var query = new ProductQueryPaginateSortable
            {
                Name = "duc",
                SortBy = "Id.DESC",
                Paginate = true
            };

            var result = Products.BuildDynamicQuery(query, x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name
            });

            result.Should().BeOfType<PagedResponse<ProductDto>>();
            var productList = result as PagedResponse<ProductDto>;
            productList.Items.Should().HaveCount(3);
        }

        private IQueryable<Product> Products => new List<Product> 
        {
            new Product{ Id = 1, Name = "Product1" },
            new Product{ Id = 2, Name = "Product2" },
            new Product{ Id = 3, Name = "Product3" },
            new Product{ Id = 4, Name = "Auto" },
            new Product{ Id = 5, Name = "Kola" },
            new Product{ Id = 6, Name = "Bla bla" },
            new Product{ Id = 7, Name = "Neka vrednost" },
        }.AsQueryable();


        private class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        private class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        private class ProductQuery
        {
            [QueryProperty(DynamicQueryBuilder.ComparisonOperator.Contains)]
            public string Name { get; set; }
        }

        private class ProductQueryPaginateSortable : SortablePagedSearch
        {
            [QueryProperty(DynamicQueryBuilder.ComparisonOperator.Contains)]
            public string Name { get; set; }
        }
    }
}
