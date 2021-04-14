using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CategoryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class CategoryQueryProcessor : ICategoryQueryProcessor
    {
        IUnitOfWork unitOfWork;

        public CategoryQueryProcessor(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Category> Create(DtoCategoryPost dtoCategoryPost)
        {
            var category = new Category()
            {
                CategoryName = dtoCategoryPost.CategoryName,
                Description = dtoCategoryPost.Description,
                Picture = dtoCategoryPost.Picture,
            };

            unitOfWork.Add(category);
            await unitOfWork.CommitAsync();

            var newCategory = await unitOfWork.Query<Category>().LastAsync();

            return newCategory;
        }

        public async Task Delete(int id)
        {
            var category = await unitOfWork.Query<Category>().FirstAsync(c => c.CategoryID == id);

            if (category == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(category);
            unitOfWork.Commit();
        }

        public IQueryable<Category> Get()
        {
            return unitOfWork.Query<Category>();
        }

        public async Task<DtoCategoryGet> GetById(int id)
        {
            var category = await unitOfWork.Query<Category>().FirstAsync(c => c.CategoryID == id);

            if (category == null)
                throw new KeyNotFoundException();

            var categoryDto = new DtoCategoryGet()
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Picture = category.Picture,
                Products = category.Products
            };

            return categoryDto;
        }

        public async Task<Category> Update(int id, DtoCategoryPut dtoCategoryPut)
        {
            var category = await unitOfWork.Query<Category>().FirstAsync(c => c.CategoryID == id);

            if (category == null)
                throw new KeyNotFoundException();

            category.Description = dtoCategoryPut.Description;
            category.Picture = dtoCategoryPut.Picture;

            unitOfWork.Commit();

            return category;
        }
    }
}
