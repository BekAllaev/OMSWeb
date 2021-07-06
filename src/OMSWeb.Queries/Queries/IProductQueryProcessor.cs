using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ProductDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public interface IProductQueryProcessor
    {
        IQueryable<Product> Get();

        Task<Product> GetById(int id);

        Task<Product> Update(int id, DtoProductPut dtoProductPut);

        Task<Product> Create(DtoProductPost dtoProductPost);

        Task Delete(int id);
    }
}
