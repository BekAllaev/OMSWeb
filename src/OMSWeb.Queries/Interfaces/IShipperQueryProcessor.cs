using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ShipperDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Interfaces
{
    public interface IShipperQueryProcessor
    {
        IQueryable<Shipper> Get();

        Task<Shipper> GetById(int id); 
        
        Task<Shipper> Update(int id, DtoShipperPut dtoShipperPut);

        Task<Shipper> Create(DtoShipperPost dtoShipperPost);

        Task Delete(int id);

    }
}
