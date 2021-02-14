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
        public Task<Order> Create(WriteOrderDTO model)
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

        public Order GetWithDetails(int id)
        {
            throw new NotImplementedException();
        }

        public NotDetailedOrderDTO GetWithoutDetails(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> Update(int id, WriteOrderDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
