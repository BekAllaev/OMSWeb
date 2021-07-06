using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.SupplierDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public interface ISupplierQueryProcessor
    {
        IQueryable<Supplier> Get();

        Task<Supplier> GetById(int id);

        Task<Supplier> Update(int id, DtoSupplierPut dtoSupplierPut);

        Task<Supplier> Create(DtoSupplierPost dtoSupplierPost);

        Task Delete(int id);
    }
}
