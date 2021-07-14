using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ShipperDto;
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
    public class ShipperQueryProcessor : IShipperQueryProcessor
    {
        IUnitOfWork unitOfWork;
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Shipper)}";
        readonly CacheTech cacheTech = CacheTech.Memory;


        public ShipperQueryProcessor(IUnitOfWork unitOfWork, Func<CacheTech, ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<Shipper> Create(DtoShipperPost dtoShipperPost)
        {
            var shipper = new Shipper()
            {
                CompanyName = dtoShipperPost.CompanyName,
                Phone = dtoShipperPost.Phone
            };

            unitOfWork.Add(shipper);

            unitOfWork.Commit();

            var newShipper = await unitOfWork.Query<Shipper>().OrderBy(x => x.ShipperID).LastAsync();

            BackgroundJob.Enqueue(() => RefreshCache());

            return newShipper;
        }

        public async Task Delete(int id)
        {
            var shipper = await unitOfWork.Query<Shipper>().FirstOrDefaultAsync(s => s.ShipperID == id);

            if (shipper == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(shipper);

            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Shipper> Get()
        {
            if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Shipper> cachedList))
            {
                cachedList = unitOfWork.Query<Shipper>();
                cacheService(cacheTech).Set(cacheKey, cachedList.ToList());
            }

            return cachedList;
        }

        public async Task<Shipper> GetById(int id)
        {
            var shipper = await unitOfWork.Query<Shipper>().FirstOrDefaultAsync(s => s.ShipperID == id);

            if (shipper == null)
                throw new KeyNotFoundException();

            return shipper;
        }

        public async Task<Shipper> Update(int id, DtoShipperPut dtoShipperPut)
        {
            var shipper = await unitOfWork.Query<Shipper>().FirstOrDefaultAsync(s => s.ShipperID == id);

            if (shipper == null)
                throw new KeyNotFoundException();

            shipper.CompanyName = dtoShipperPut.CompanyName;
            shipper.Phone = dtoShipperPut.Phone;

            unitOfWork.Commit();

            BackgroundJob.Enqueue(() => RefreshCache());

            return shipper;
        }

        public async Task RefreshCache()
        {
            cacheService(cacheTech).Remove(cacheKey);
            var list = await unitOfWork.Query<Shipper>().ToListAsync();
            cacheService(cacheTech).Set(cacheKey, list);
        }
    }
}
