using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.EmployeeDto;
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
    public class EmployeeQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Employee> employees;
        private readonly IEmployeeQueryProcessor employeeQueryProcessor;

        public EmployeeQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            employees = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = employees.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Employee>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Employee>())).Callback<Employee>(employee => employees.Remove(employee));
            unitOfWork.Setup(x => x.Add(It.IsAny<Employee>())).Callback<Employee>(employee => employees.Add(employee));
            unitOfWork.Setup(x => x.Update(It.IsAny<Employee>())).Callback<Employee>(employee =>
            {
                var item = employees.Find(x => x.EmployeeID == employee.EmployeeID);

                item = employee;
            });

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            employeeQueryProcessor = new EmployeeQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            employees.Add(new Employee() { EmployeeID = 1, FirstName = "John" });

            var result = employeeQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            employees.Add(new Employee() { EmployeeID = 1, FirstName = "John" });
            employees.Add(new Employee() { EmployeeID = 2, FirstName = "Jack" });

            var id = employees[0].EmployeeID;

            var result = await employeeQueryProcessor.GetById(id);
            result.EmployeeID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await employeeQueryProcessor.GetById(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            int id = 1;

            employees.Add(new Employee() { EmployeeID = id });

            await employeeQueryProcessor.Delete(id);

            employees.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await employeeQueryProcessor.Delete(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var employee = await employeeQueryProcessor.Create(new DtoEmployeePost()
            {
                FirstName = "John"
            });

            employees.Should().Contain(employee);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public async Task UpdateShouldEditPropertyValue()
        {
            int id = 1;

            employees.Add(new Employee() { EmployeeID = id, FirstName = "John", Address = "Uzbekistan" });

            string newAddress = "New Uzbekistan";

            var employee = await employeeQueryProcessor.Update(id, new DtoEmployeePut() { Address = newAddress });

            employee.Address.Should().Be(newAddress);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void UpdateShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await employeeQueryProcessor.Update(1, new DtoEmployeePut());

            func.Should().Throw<KeyNotFoundException>();
        }
    }
}
