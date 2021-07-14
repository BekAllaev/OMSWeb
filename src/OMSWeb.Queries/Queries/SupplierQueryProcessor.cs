using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.SupplierDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class SupplierQueryProcessor : ISupplierQueryProcessor
    {
        IUnitOfWork unitOfWork;
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Supplier)}";
        readonly CacheTech cacheTech = CacheTech.Memory;

        public SupplierQueryProcessor(IUnitOfWork unitOfWork, Func<CacheTech, ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<Supplier> Create(DtoSupplierPost dtoSupplierPost)
        {
            var supplier = new Supplier()
            {
                CompanyName = dtoSupplierPost.CompanyName,
                ContactName = dtoSupplierPost.ContactName,
                ContactTitle = dtoSupplierPost.ContactTitle,
                Address = dtoSupplierPost.Address,
                City = dtoSupplierPost.City,
                Region = dtoSupplierPost.Region,
                PostalCode = dtoSupplierPost.PostalCode,
                Country = dtoSupplierPost.Country,
                Phone = dtoSupplierPost.Phone,
                Fax = dtoSupplierPost.Fax,
                HomePage = dtoSupplierPost.HomePage,
            };

            unitOfWork.Add(supplier);

            unitOfWork.Commit();

            var newSupplier = await unitOfWork.Query<Supplier>().OrderBy(x => x.SupplierID).LastAsync();

            //BackgroundJob.Enqueue(() => RefreshCache());

            return newSupplier;
        }

        public async Task Delete(int id)
        {
            var supplier = await unitOfWork.Query<Supplier>().FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(supplier);
            unitOfWork.Commit();

            //BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Supplier> Get()
        {
            //if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Supplier> cachedList))
            //{
            //    cachedList = unitOfWork.Query<Supplier>();
            //    cacheService(cacheTech).Set(cacheKey, cachedList);
            //}

            //return cachedList;

            return unitOfWork.Query<Supplier>();
        }

        public async Task<Supplier> GetById(int id)
        {
            var supplier = await unitOfWork.Query<Supplier>().FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
                throw new KeyNotFoundException();

            return supplier;
        }

        public async Task<Supplier> Update(int id, DtoSupplierPut dtoSupplierPut)
        {
            var supplier = await unitOfWork.Query<Supplier>().FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
                throw new KeyNotFoundException();

            supplier.CompanyName = dtoSupplierPut.CompanyName;
            supplier.ContactName = dtoSupplierPut.ContactName;
            supplier.ContactTitle = dtoSupplierPut.ContactTitle;
            supplier.Address = dtoSupplierPut.Address;
            supplier.City = dtoSupplierPut.City;
            supplier.Region = dtoSupplierPut.Region;
            supplier.PostalCode = dtoSupplierPut.PostalCode;
            supplier.Country = dtoSupplierPut.Country;
            supplier.Phone = dtoSupplierPut.Phone;
            supplier.Fax = dtoSupplierPut.Fax;
            supplier.HomePage = dtoSupplierPut.HomePage;

            unitOfWork.Update(supplier);

            unitOfWork.Commit();

            //BackgroundJob.Enqueue(() => RefreshCache());

            return supplier;
        }

        //public async Task RefreshCache()
        //{
        //    cacheService(cacheTech).Remove(cacheKey);
        //    var cachedList = await unitOfWork.Query<Supplier>().ToListAsync();
        //    cacheService(cacheTech).Set(cacheKey, cachedList);
        //}
    }
}
