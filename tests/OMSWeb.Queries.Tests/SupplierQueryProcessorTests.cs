using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.SupplierDto;
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
    public class SupplierQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Supplier> suppliers;
        private readonly ISupplierQueryProcessor supplierQueryProcessor;

        public SupplierQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            suppliers = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = suppliers.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Supplier>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Supplier>())).Callback<Supplier>(supplier => suppliers.Remove(supplier));
            unitOfWork.Setup(x => x.Add(It.IsAny<Supplier>())).Callback<Supplier>(supplier => suppliers.Add(supplier));
            unitOfWork.Setup(x => x.Update(It.IsAny<Supplier>())).Callback<Supplier>(supplier =>
            {
                var item = suppliers.Find(x => x.SupplierID == supplier.SupplierID);

                item = supplier;
            });

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            supplierQueryProcessor = new SupplierQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            suppliers.Add(new Supplier() { SupplierID = 1, CompanyName = "Supplier1" });

            var result = supplierQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            suppliers.Add(new Supplier() { SupplierID = 1, CompanyName = "Supplier1" });
            suppliers.Add(new Supplier() { SupplierID = 2, CompanyName = "Supplier2" });

            var id = suppliers[0].SupplierID;

            var result = await supplierQueryProcessor.GetById(id);
            result.SupplierID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await supplierQueryProcessor.GetById(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            int id = 1;

            suppliers.Add(new Supplier() { SupplierID = id });

            await supplierQueryProcessor.Delete(id);

            suppliers.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await supplierQueryProcessor.Delete(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var Supplier = await supplierQueryProcessor.Create(new DtoSupplierPost()
            {
                CompanyName = "Supplier1"
            });

            suppliers.Should().Contain(Supplier);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public async Task UpdateShouldEditPropertyValue()
        {
            int id = 1;

            suppliers.Add(new Supplier() { SupplierID = id, CompanyName = "Supplier1", Phone = "123-45-67" });

            string newPhone = "123-45-67-89";

            var supplier = await supplierQueryProcessor.Update(id, new DtoSupplierPut() { Phone = newPhone });

            supplier.Phone.Should().Be(newPhone);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void UpdateShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await supplierQueryProcessor.Update(1, new DtoSupplierPut());

            func.Should().Throw<KeyNotFoundException>();
        }

    }
}
