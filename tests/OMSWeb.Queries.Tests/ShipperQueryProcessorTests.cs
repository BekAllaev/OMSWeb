using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ShipperDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Interfaces;
using OMSWeb.Queries.Queries;
using OMSWeb.Queries.Tests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OMSWeb.Queries.Tests
{
    public class ShipperQueryProcessorTests
    {
        private Mock<IUnitOfWork> unitOfWork;

        private List<Shipper> shippers;
        private IShipperQueryProcessor shipperQueryProcessor;

        public ShipperQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            shippers = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = shippers.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Shipper>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Shipper>())).Callback<Shipper>(shipper => shippers.Remove(shipper));
            unitOfWork.Setup(x => x.Add(It.IsAny<Shipper>())).Callback<Shipper>(shipper => shippers.Add(shipper));
            unitOfWork.Setup(x => x.Update(It.IsAny<Shipper>())).Callback<Shipper>(shipper =>
            {
                var item = shippers.Find(x => x.ShipperID == shipper.ShipperID);

                item = shipper;
            });

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            shipperQueryProcessor = new ShipperQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            shippers.Add(new Shipper() { ShipperID = 1, CompanyName = "Shipper1" });

            var result = shipperQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            shippers.Add(new Shipper() { ShipperID = 1, CompanyName = "Shipper1" });
            shippers.Add(new Shipper() { ShipperID = 2, CompanyName = "Shipper2" });

            var id = shippers[0].ShipperID;

            var result = await shipperQueryProcessor.GetById(id);
            result.ShipperID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () =>
            {
                await shipperQueryProcessor.GetById(1);
            };

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            int id = 1;

            shippers.Add(new Shipper() { ShipperID = id });

            await shipperQueryProcessor.Delete(id);

            shippers.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await shipperQueryProcessor.Delete(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var Shipper = await shipperQueryProcessor.Create(new DtoShipperPost()
            {
                CompanyName = "Shipper1"
            });

            shippers.Should().Contain(Shipper);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public async Task UpdateShouldEditPropertyValue()
        {
            int id = 1;

            shippers.Add(new Shipper() { ShipperID = id, CompanyName = "Shipper1", Phone="123-45-67"});

            string newPhone = "123-45-67-89";

            var shipper = await shipperQueryProcessor.Update(id, new DtoShipperPut() { Phone = newPhone });

            shipper.Phone.Should().Be(newPhone);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void UpdateShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await shipperQueryProcessor.Update(1, new DtoShipperPut());

            func.Should().Throw<KeyNotFoundException>();
        }
    }
}
