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

        Task<Order> GetById(int id);

        Task<DtoOrderGetWithDetails> GetWithDetails(int id);

        Task<DtoOrderGetWithoutDetails> GetWithoutDetails(int id);

        Task<Order> Create(DtoOrderPost orderDto);

        Task Delete(int id);
    }
}
