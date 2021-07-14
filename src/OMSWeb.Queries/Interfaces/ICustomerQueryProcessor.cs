using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CutomerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Interfaces
{
    public interface ICustomerQueryProcessor
    {
        IQueryable<Customer> Get();

        Task<Customer> GetById(string id);

        Task<Customer> Update(string id, DtoCustomerPutPost dtoCustomerPut);

        Task<Customer> Create(DtoCustomerPutPost dtoCustomerPost);

        Task Delete(string id);
    }
}
