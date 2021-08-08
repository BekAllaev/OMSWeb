using OMSWeb.Dto.Model.OrderDto;
using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMSWeb.Data.Access.DAL;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using Hangfire;
using OMSWeb.Queries.Interfaces;
using OMSWeb.Queries.Extensions;

namespace OMSWeb.Queries.Queries
{
    public class OrderQueryProcessor : IOrderQueryProcessor
    {
        IUnitOfWork unitOfWork;
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Order)}";
        readonly CacheTech cacheTech = CacheTech.Memory;

        public OrderQueryProcessor(IUnitOfWork unitOfWork, Func<CacheTech, ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<Order> Create(DtoOrderPost orderDto)
        {
            Order order = new Order()
            {
                CustomerID = orderDto.CustomerID,
                EmployeeID = orderDto.EmployeeID,
                ShipAddress = orderDto.ShipAddress,
                ShipCity = orderDto.ShipCity,
                ShipCountry = orderDto.ShipCountry,
                ShipName = orderDto.ShipName,
                ShippedDate = orderDto.ShippedDate,
                ShipPostalCode = orderDto.ShipPostalCode,
                ShipRegion = orderDto.ShipRegion,
                ShipVia = orderDto.ShipVia,
                OrderDate = orderDto.OrderDate,
                Freight = orderDto.Freight,
                RequiredDate = orderDto.RequiredDate
            };

            using (var transaction = unitOfWork.BeginTransaction())
            {
                try
                {
                    unitOfWork.Add(order);
                    await unitOfWork.CommitAsync();

                    var id = (await unitOfWork.Query<Order>().LastAsync()).OrderID;

                    foreach (var orderDetail in orderDto.Order_Details)
                    {
                        orderDetail.OrderID = id;
                        orderDetail.ProductID = orderDto.ProductID;
                        unitOfWork.Add(orderDetail);
                    }
                    await unitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                }
            }

            Order resultOrder = await unitOfWork.Query<Order>().OrderBy(x => x.OrderID).LastAsync();

            BackgroundJob.Enqueue(() => RefreshCache());

            return resultOrder;
        }

        public async Task Delete(int id)
        {
            var order = await unitOfWork.Query<Order>().FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(order);

            unitOfWork.Commit();

            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Order> Get()
        {
            var result = cacheService(cacheTech).GetCacheOrQuery<Order>(unitOfWork, cacheKey);

            return result;
        }

        public async Task<Order> GetById(int id)
        {
            //Business logic: When get one order we need to know its details. Return value acts like invoice
            var order = await unitOfWork.Query<Order>().Include(order => order.Order_Details).FirstOrDefaultAsync(o => o.OrderID == id);

            if (order is null)
                throw new KeyNotFoundException();

            return order;
        }

        public async Task RefreshCache()
        {
            cacheService(cacheTech).Remove(cacheKey);

            var orders = await unitOfWork.Query<Order>().ToListAsync();

            cacheService(cacheTech).Set(cacheKey, orders);
        }
    }
}
