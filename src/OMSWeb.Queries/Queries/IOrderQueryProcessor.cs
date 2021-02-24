using OMSWeb.Dto.Model.Order;
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

        DtoOrderGetWithDetails GetWithDetails(int id);

        DtoOrderGetWithoutDetails GetWithoutDetails(int id);

        Task<Order> Create(DtoOrderPutPost orderDto);

        Task<Order> Update(int id, DtoOrderPutPost orderDto);

        Task Delete(int id);
    }
}
