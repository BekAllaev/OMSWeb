using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CutomerDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Interfaces;
using OMSWeb.Queries.Queries;
using OMSWeb.Queries.Tests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OMSWeb.Queries.Tests
{
    public class CustomerQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Customer> customers;
        private readonly ICustomerQueryProcessor customerQueryProcessor;

        public CustomerQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            customers = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = customers.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Customer>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Customer>())).Callback<Customer>(customer => customers.Remove(customer));
            unitOfWork.Setup(x => x.Add(It.IsAny<Customer>())).Callback<Customer>(customer => customers.Add(customer));
            unitOfWork.Setup(x => x.Update(It.IsAny<Customer>())).Callback<Customer>(customer =>
            {
                var item = customers.Find(x => x.CustomerID == customer.CustomerID);

                item = customer;
            });

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            customerQueryProcessor = new CustomerQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            customers.Add(new Customer() { CustomerID = "ABC", CompanyName = "Customer1" });

            var result = customerQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            customers.Add(new Customer() { CustomerID = "ABC", CompanyName = "Customer1" });
            customers.Add(new Customer() { CustomerID = "ABC", CompanyName = "Customer2" });

            var id = customers[0].CustomerID;

            var result = await customerQueryProcessor.GetById(id);
            result.CustomerID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await customerQueryProcessor.GetById("ABC");

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            string id = "ABC";

            customers.Add(new Customer() { CustomerID = id });

            await customerQueryProcessor.Delete(id);

            customers.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await customerQueryProcessor.Delete("ABC");

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var customer = await customerQueryProcessor.Create(new DtoCustomerPutPost()
            {
                CompanyName = "Supplier1"
            });

            customers.Should().Contain(customer);

            unitOfWork.Verify(x => x.CommitAsync());
        }

        [Fact]
        public async Task UpdateShouldEditPropertyValue()
        {
            string id = "ABC";

            customers.Add(new Customer() { CustomerID = id, CompanyName = "Supplier1", Phone = "123-45-67" });

            string newPhone = "123-45-67-89";

            var customer = await customerQueryProcessor.Update(id, new DtoCustomerPutPost() { Phone = newPhone });

            customer.Phone.Should().Be(newPhone);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void UpdateShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await customerQueryProcessor.Update("ABC", new DtoCustomerPutPost());

            func.Should().Throw<KeyNotFoundException>();
        }
    }
}
