using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ProductDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class ProductQueryProcessor : IProductQueryProcessor
    {
        IUnitOfWork unitOfWork;
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Product)}";
        readonly CacheTech cacheTech = CacheTech.Memory;


        public ProductQueryProcessor(IUnitOfWork unitOfWork, Func<CacheTech,ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<Product> Create(DtoProductPost dtoProductPost)
        {
            var product = new Product()
            {
                CategoryID = dtoProductPost.CategoryID,
                SupplierID = dtoProductPost.SupplierID,
                ProductName = dtoProductPost.ProductName,
                UnitPrice = dtoProductPost.UnitPrice,
                QuantityPerUnit = dtoProductPost.QuantityPerUnit,
                Discontinued = dtoProductPost.Discontinued,
                UnitsInStock = dtoProductPost.UnitsInStock,
                UnitsOnOrder = dtoProductPost.UnitsOnOrder,
                ReorderLevel = dtoProductPost.ReorderLevel
            };

            unitOfWork.Add(product);

            unitOfWork.Commit();

            var newProduct = await unitOfWork.Query<Product>().OrderBy(x => x.ProductID).LastOrDefaultAsync();

            //BackgroundJob.Enqueue(() => RefreshCache());

            return newProduct;
        }

        public async Task Delete(int id)
        {
            var product = await unitOfWork.Query<Product>().FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(product);
            unitOfWork.Commit();

            //BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Product> Get()
        {
            //if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Product> cachedList))
            //{
            //    cachedList = unitOfWork.Query<Product>();
            //    cacheService(cacheTech).Set(cacheKey, cachedList.ToList());
            //}

            //return cachedList;

            return unitOfWork.Query<Product>();
        }

        public async Task<Product> GetById(int id)
        {
            var product = await unitOfWork.Query<Product>().FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new KeyNotFoundException();

            return product;
        }

        public async Task<Product> Update(int id, DtoProductPut dtoProductPut)
        {
            var product = await unitOfWork.Query<Product>().FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new KeyNotFoundException();

            product.SupplierID = dtoProductPut.SupplierID;
            product.QuantityPerUnit = dtoProductPut.QuantityPerUnit;
            product.UnitPrice = dtoProductPut.UnitPrice;
            product.UnitsInStock = dtoProductPut.UnitsInStock;
            product.UnitsOnOrder = dtoProductPut.UnitsOnOrder;
            product.ReorderLevel = dtoProductPut.ReorderLevel;
            product.Discontinued = dtoProductPut.Discontinued;

            unitOfWork.Update(product);

            unitOfWork.Commit();

            //BackgroundJob.Enqueue(() => RefreshCache());

            return product;
        }

        //public async Task RefreshCache()
        //{
        //    cacheService(cacheTech).Remove(cacheKey);
        //    var cachedList = await unitOfWork.Query<Product>().ToListAsync();
        //    cacheService(cacheTech).Set(cacheKey, cachedList);
        //}
    }
}
