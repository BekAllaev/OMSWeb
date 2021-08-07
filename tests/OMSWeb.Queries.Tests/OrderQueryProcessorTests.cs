﻿using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
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

namespace OMSWeb.Queries.Tests
{
    public class OrderQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Order> orders;
        private readonly IOrderQueryProcessor orderQueryProcessor;

        public OrderQueryProcessorTests()
        {
            ICacheService func(CacheTech cacheTech) => new Mock<ICacheService>().Object;

            unitOfWork = new();
            orders = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = orders.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Order>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Order>())).Callback<Order>(order => orders.Remove(order));
            unitOfWork.Setup(x => x.Add(It.IsAny<Order>())).Callback<Order>(order => orders.Add(order));

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            orderQueryProcessor = new OrderQueryProcessor(unitOfWork.Object, func);
        }


    }
}