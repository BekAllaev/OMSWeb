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

                    foreach (var orderDetail in orderDto.Order_Details.ToList())
                    {
                        unitOfWork.Add(orderDetail);
                    }

                    await unitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                }
            }

            Order resultOrder = await unitOfWork.Query<Order>().LastAsync();

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
            if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Order> cachedList))
            {
                cachedList = unitOfWork.Query<Order>();
                cacheService(cacheTech).Set(cacheKey, cachedList);
            }

            return cachedList;
        }

        public async Task<Order> GetById(int id)
        {
            var order = await unitOfWork.Query<Order>().FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
                throw new KeyNotFoundException();

            return order;
        }

        public async Task<DtoOrderGetWithDetails> GetWithDetails(int id)
        {
            var order = await unitOfWork.Query<Order>().FirstAsync(o => o.OrderID == id);

            DtoOrderGetWithDetails orderWithDetails = new DtoOrderGetWithDetails()
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                EmployeeID = order.EmployeeID,
                ShipAddress = order.ShipAddress,
                ShipCity = order.ShipCity,
                ShipCountry = order.ShipCountry,
                ShipName = order.ShipName,
                ShippedDate = order.ShippedDate,
                ShipPostalCode = order.ShipPostalCode,
                ShipRegion = order.ShipRegion,
                ShipVia = order.ShipVia,
                Freight = order.Freight,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                Order_Details = order.Order_Details
            };

            return orderWithDetails;
        }

        public async Task<DtoOrderGetWithoutDetails> GetWithoutDetails(int id)
        {
            var order = await unitOfWork.Query<Order>().FirstAsync(o => o.OrderID == id);

            DtoOrderGetWithoutDetails orderWithoutDetails = new DtoOrderGetWithoutDetails()
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                EmployeeID = order.EmployeeID,
                ShipAddress = order.ShipAddress,
                ShipCity = order.ShipCity,
                ShipCountry = order.ShipCountry,
                ShipName = order.ShipName,
                ShippedDate = order.ShippedDate,
                ShipPostalCode = order.ShipPostalCode,
                ShipRegion = order.ShipRegion,
                ShipVia = order.ShipVia,
                Freight = order.Freight,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
            };

            return orderWithoutDetails;
        }

        public async Task RefreshCache()
        {
            cacheService(cacheTech).Remove(cacheKey);
            cacheService(cacheTech).Remove($"{typeof(Order_Detail)}");

            var cachedOrders = await unitOfWork.Query<Order>().ToListAsync();
            var cachedOrderDetails = await unitOfWork.Query<Order_Detail>().ToListAsync();

            cacheService(cacheTech).Set(cacheKey, cachedOrders);
            cacheService(cacheTech).Set(cacheKey, cachedOrderDetails);
        }
    }
}
