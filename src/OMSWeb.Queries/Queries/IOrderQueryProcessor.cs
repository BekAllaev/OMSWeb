using OMSWeb.Dto.Model.OrderDto;
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

        OrderDtoWithDetails GetWithDetails(int id);

        OrderDtoWithoutDetails GetWithoutDetails(int id);

        Task<Order> Create(OrderDtoForPutPost orderDto);

        Task<Order> Update(int id, OrderDtoForPutPost orderDto);

        Task Delete(int id);
    }
}
