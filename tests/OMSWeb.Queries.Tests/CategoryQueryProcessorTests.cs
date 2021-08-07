using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CategoryDto;
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
    public class CategoryQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Category> categories;
        private readonly ICategoryQueryProcessor categoryQueryProcessor;

        public CategoryQueryProcessorTests()
        {
            Func<CacheTech, ICacheService> func = cacheTech => new Mock<ICacheService>().Object;

            unitOfWork = new();
            categories = new();

            //Look this link to see what BuildMock method do
            //https://github.com/romantitov/MockQueryable#how-do-i-get-started
            var mock = categories.AsQueryable().BuildMock();

            unitOfWork.Setup(x => x.Query<Category>()).Returns(mock.Object);
            unitOfWork.Setup(x => x.Delete(It.IsAny<Category>())).Callback<Category>(category => categories.Remove(category));
            unitOfWork.Setup(x => x.Add(It.IsAny<Category>())).Callback<Category>(category => categories.Add(category));
            unitOfWork.Setup(x => x.Update(It.IsAny<Category>())).Callback<Category>(category =>
            {
                var item = categories.Find(x => x.CategoryID == category.CategoryID);

                item = category;
            });

            JobStorage.Current = new SqlServerStorage(ConfigExtensions.GetConfiguration().GetConnectionString("SqlConnection"));

            categoryQueryProcessor = new CategoryQueryProcessor(unitOfWork.Object, func);
        }

        [Fact]
        public void GetShouldReturnAll()
        {
            categories.Add(new Category() { CategoryID = 1, CategoryName = "Category1" });

            var result = categoryQueryProcessor.Get().ToList();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdShouldReturnCorrectInstance()
        {
            categories.Add(new Category() { CategoryID = 1, CategoryName = "Category1" });
            categories.Add(new Category() { CategoryID = 2, CategoryName = "Category2" });

            var id = categories[0].CategoryID;

            var result = await categoryQueryProcessor.GetById(id);
            result.CategoryID.Should().Be(id);
        }

        [Fact]
        public void GetByIdShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await categoryQueryProcessor.GetById(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteShouldRemoveItem()
        {
            int id = 1;

            categories.Add(new Category() { CategoryID = id });

            await categoryQueryProcessor.Delete(id);

            categories.Should().HaveCount(0);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void DeleteShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await categoryQueryProcessor.Delete(1);

            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateShouldSaveNew()
        {
            var category = await categoryQueryProcessor.Create(new DtoCategoryPost()
            {
                CategoryName = "Category1"
            });

            categories.Should().Contain(category);

            unitOfWork.Verify(x => x.CommitAsync());
        }

        [Fact]
        public async Task UpdateShouldEditPropertyValue()
        {
            int id = 1;

            categories.Add(new Category() { CategoryID = id, CategoryName = "Product1", Description = "Description" });

            string newDescription = "Description1";

            var category = await categoryQueryProcessor.Update(id, new DtoCategoryPut() { Description = newDescription });

            category.Description.Should().Be(newDescription);

            unitOfWork.Verify(x => x.Commit());
        }

        [Fact]
        public void UpdateShouldThrowExceptionIfNotFound()
        {
            Func<Task> func = async () => await categoryQueryProcessor.Update(1, new DtoCategoryPut());

            func.Should().Throw<KeyNotFoundException>();
        }
    }
}
