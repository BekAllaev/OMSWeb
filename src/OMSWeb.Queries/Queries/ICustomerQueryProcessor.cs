using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CutomerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public interface ICustomerQueryProcessor
    {
        IQueryable<Customer> Get();

        Task<DtoCustomerGet> GetById(int id);

        Task<Customer> Update(int id, DtoCustomerPutPost dtoCustomerPut);

        Task<Customer> Create(DtoCustomerPutPost dtoCustomerPost);

        Task Delete(int id);
    }
}
