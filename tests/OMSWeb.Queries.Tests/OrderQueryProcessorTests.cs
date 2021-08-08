using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.OrderDto;
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
    public class OrderQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Order> orders;
        private readonly List<Order_Detail> orderDetails;
        private readonly IOrderQueryProcessor orderQueryProcessor;

        public OrderQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            orders = new();
            orderDetails = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var orderMock = orders.AsQueryable().BuildMock();
            var orderDetailsMock = orderDetails.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Order>()).Returns(orderMock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Order>())).Callback<Order>(order => orders.Remove(order));
            unitOfWork.Setup(x => x.Add(It.IsAny<Order>())).Callback<Order>(order => 
            {
                order.OrderID = 1;
                orders.Add(order); 
            });
            unitOfWork.Setup(x => x.Add(It.IsAny<Order_Detail>())).Callback<Order_Detail>(orderDetail => orderDetails.Add(orderDetail));

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            orderQueryProcessor = new OrderQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            orders.Add(new Order());

            var result = orderQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            orders.Add(new Order() { OrderID = 1 });
            orders.Add(new Order() { OrderID = 2 });

            var id = orders[0].OrderID;

            var result = await orderQueryProcessor.GetById(id);
            result.OrderID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await orderQueryProcessor.GetById(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            int id = 1;

            orders.Add(new Order() { OrderID = id });

            await orderQueryProcessor.Delete(id);

            orders.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await orderQueryProcessor.Delete(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var order = await orderQueryProcessor.Create(new DtoOrderPost()
            {
                ProductID = 1,
                Order_Details = Enumerable.Empty<Order_Detail>()
            });

            orders.Should().Contain(order);

            unitOfWork.Verify(x => x.CommitAsync());
        }

        [Fact]
        public async Task CreateShouldSaveOrderDetails()
        {
            var productId = 1;

            var order = await orderQueryProcessor.Create(new DtoOrderPost()
            {
                ProductID = productId,
                Order_Details = new List<Order_Detail>()
                {
                    new Order_Detail(){ ProductID = productId }
                }
            });

            orderDetails.First().Should().BeEquivalentTo(new 
            { 
                OrderID = order.OrderID, 
                ProductID = productId 
            }, options => options.ExcludingMissingMembers());

            unitOfWork.Verify(x => x.CommitAsync(), Times.Exactly(2));
        }
    }
}
