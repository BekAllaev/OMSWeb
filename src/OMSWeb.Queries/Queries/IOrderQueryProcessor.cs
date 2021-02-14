using OMSWeb.API.Models.Orders;
using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public interface IOrderQueryProcessor
    {
        IQueryable<Order> Get();

        Order GetWithDetails(int id);

        NotDetailedOrderDTO GetWithoutDetails(int id);

        Task<Order> Create(WriteOrderDTO model);

        Task<Order> Update(int id, WriteOrderDTO model);

        Task Delete(int id);
    }
}
