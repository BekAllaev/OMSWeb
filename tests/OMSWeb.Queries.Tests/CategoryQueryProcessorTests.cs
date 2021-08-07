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
    }
}
