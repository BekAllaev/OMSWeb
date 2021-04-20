using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CategoryDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
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
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Category)}";
        readonly CacheTech cacheTech = CacheTech.Memory;

        public CategoryQueryProcessor(IUnitOfWork unitOfWork, Func<CacheTech, ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
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

            BackgroundJob.Enqueue(() => RefreshCache());

            return newCategory;
        }

        public async Task Delete(int id)
        {
            var category = await unitOfWork.Query<Category>().FirstAsync(c => c.CategoryID == id);

            if (category == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(category);
            unitOfWork.Commit();

            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Category> Get()
        {
            if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Category> cachedList))
            {
                cachedList = unitOfWork.Query<Category>();
                cacheService(cacheTech).Set(cacheKey, cachedList);
            }

            return cachedList;
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

            BackgroundJob.Enqueue(() => RefreshCache());

            return category;
        }

        public async Task RefreshCache()
        {
            cacheService(cacheTech).Remove(cacheKey);
            var cachedList = await unitOfWork.Query<Category>().ToListAsync();
            cacheService(cacheTech).Set(cacheKey, cachedList);
        }
    }
}
