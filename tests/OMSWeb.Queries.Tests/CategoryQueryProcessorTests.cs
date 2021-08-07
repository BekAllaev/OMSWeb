using Moq;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Queries.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Tests
{
    public class CategoryQueryProcessorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;

        private readonly List<Category> categories;
        private readonly ICategoryQueryProcessor categoryQueryProcessor;

        public CategoryQueryProcessorTests()
        {

        }
    }
}
