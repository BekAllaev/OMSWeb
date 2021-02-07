using OMSWeb.API.Models.Orders;
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
        public Task<Order> Create(CreateOrderDTO model)
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

        public Order Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> Update(int id, UpdateOrderDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
