using Microsoft.EntityFrameworkCore;
using FluentAssertions; //FluentAssertions
using MockQueryable.Moq; //MockQueryable.Moq
using Moq; //Moq
using Xunit; //Xunit
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Interfaces;
using OMSWeb.Queries.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using OMSWeb.Dto.Model.ProductDto;
using OMSWeb.Queries.Tests.Configuration;
using Microsoft.Extensions.Configuration;

namespace OMSWeb.Queries.Tests
{
    public class ProductQueryProcessorTests
    {
        private Mock<IUnitOfWork> unitOfWork;

        private List<Product> products;
        private IProductQueryProcessor productQueryProcessor;

        public ProductQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            products = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = products.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Product>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Product>())).Callback<Product>(product => products.Remove(product));
            unitOfWork.Setup(x => x.Add(It.IsAny<Product>())).Callback<Product>(product => products.Add(product));
            unitOfWork.Setup(x => x.Update(It.IsAny<Product>())).Callback<Product>(product =>
            {
                var item = products.Find(x => x.ProductID == product.ProductID);

                item = product;
            });

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            productQueryProcessor = new ProductQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            products.Add(new Product() { ProductID = 1, ProductName = "Product1" });

            var result = productQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            products.Add(new Product() { ProductID = 1, ProductName = "Product1" });
            products.Add(new Product() { ProductID = 2, ProductName = "Product2" });

            var id = products[0].ProductID;

            var result = await productQueryProcessor.GetById(id);
            result.ProductID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () =>
            {
                await productQueryProcessor.GetById(1);
            };

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            int id = 1;

            products.Add(new Product() { ProductID = id });

            await productQueryProcessor.Delete(id);

            products.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await productQueryProcessor.Delete(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var product = await productQueryProcessor.Create(new DtoProductPost()
            {
                ProductName = "Product1"
            });

            products.Should().Contain(product);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public async Task UpdateShouldEditPropertyValue()
        {
            int id = 1;

            products.Add(new Product() { ProductID = id, ProductName = "Product1", UnitPrice = 1 });

            int newUnitPrice = 10;

            var product = await productQueryProcessor.Update(id, new DtoProductPut() { UnitPrice = newUnitPrice });

            product.UnitPrice.Should().Be(newUnitPrice);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void UpdateShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await productQueryProcessor.Update(1, new DtoProductPut());

            func.Should().Throw<KeyNotFoundException>();
        }
    }
}
