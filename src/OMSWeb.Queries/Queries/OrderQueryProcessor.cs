using OMSWeb.Dto.Model.OrderDto;
using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class OrderQueryProcessor : IOrderQueryProcessor
    {
        public Task<Order> Create(OrderDtoForPutPost orderDto)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Order> Get()
        {
            throw new NotImplementedException();
        }

        public OrderDtoWithDetails GetWithDetails(int id)
        {
            throw new NotImplementedException();
        }

        public OrderDtoWithoutDetails GetWithoutDetails(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> Update(int id, OrderDtoForPutPost orderDto)
        {
            throw new NotImplementedException();
        }
    }
}
