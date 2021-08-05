using Hangfire;
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
    }
}
