using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CutomerDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class CustomerQueryProcessor : ICustomerQueryProcessor
    {
        IUnitOfWork unitOfWork;
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Customer)}";
        readonly CacheTech cacheTech = CacheTech.Memory;

        public CustomerQueryProcessor(IUnitOfWork unitOfWork, Func<CacheTech, ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<Customer> Create(DtoCustomerPutPost dtoCustomerPost)
        {
            var customer = new Customer()
            {
                CompanyName = dtoCustomerPost.CompanyName,
                ContactName = dtoCustomerPost.ContactName,
                ContactTitle = dtoCustomerPost.ContactTitle,
                Address = dtoCustomerPost.Address,
                City = dtoCustomerPost.City,
                Region = dtoCustomerPost.Region,
                PostalCode = dtoCustomerPost.PostalCode,
                Country = dtoCustomerPost.Country,
                Phone = dtoCustomerPost.Phone,
                Fax = dtoCustomerPost.Fax,
            };

            unitOfWork.Add(customer);
            await unitOfWork.CommitAsync();

            var newCustomer = await unitOfWork.Query<Customer>().LastAsync();

            BackgroundJob.Enqueue(() => RefreshCache());

            return newCustomer;
        }

        public async Task Delete(string id)
        {
            var customer = await unitOfWork.Query<Customer>().FirstOrDefaultAsync(c => c.CustomerID == id);

            if (customer == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(customer);
            unitOfWork.Commit();

            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Customer> Get()
        {
            if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Customer> cachedList))
            {
                cachedList = unitOfWork.Query<Customer>();
                cacheService(cacheTech).Set(cacheKey, cachedList);
            }

            return cachedList;
        }

        public async Task<DtoCustomerGet> GetById(string id)
        {
            var customer = await unitOfWork.Query<Customer>().FirstOrDefaultAsync(c => c.CustomerID == id);

            if (customer == null)
                throw new KeyNotFoundException();

            var customerDto = new DtoCustomerGet()
            {
                CustomerID = customer.CustomerID,
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                City = customer.City,
                ContactTitle = customer.ContactTitle,
                Country = customer.Country,
                PostalCode = customer.PostalCode,
                Address = customer.Address,
                Phone = customer.Phone,
                Region = customer.Region,
                Fax = customer.Fax,
                Orders = customer.Orders
            };

            return customerDto;
        }

        public async Task<Customer> Update(string id, DtoCustomerPutPost dtoCustomerPut)
        {
            var customer = await unitOfWork.Query<Customer>().FirstOrDefaultAsync(c => c.CustomerID == id);

            if (customer == null)
                throw new KeyNotFoundException();

            customer.CompanyName = dtoCustomerPut.CompanyName;
            customer.ContactName = dtoCustomerPut.ContactName;
            customer.ContactTitle = dtoCustomerPut.ContactTitle;
            customer.Address = dtoCustomerPut.Address;
            customer.City = dtoCustomerPut.City;
            customer.Region = dtoCustomerPut.Region;
            customer.PostalCode = dtoCustomerPut.PostalCode;
            customer.Country = dtoCustomerPut.Country;
            customer.Phone = dtoCustomerPut.Phone;
            customer.Fax = dtoCustomerPut.Fax;

            unitOfWork.Commit();

            BackgroundJob.Enqueue(() => RefreshCache());

            return customer;
        }

        public async Task RefreshCache()
        {
            cacheService(cacheTech).Remove(cacheKey);
            var cachedList = await unitOfWork.Query<Customer>().ToListAsync();
            cacheService(cacheTech).Set(cacheKey, cachedList);
        }
    }
}
