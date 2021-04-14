using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ShipperDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class ShipperQueryProcessor : IShipperQueryProcessor
    {
        IUnitOfWork unitOfWork;

        public ShipperQueryProcessor(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Shipper> Create(DtoShipperPost dtoShipperPost)
        {
            var shipper = new Shipper()
            {
                CompanyName = dtoShipperPost.CompanyName,
                Phone = dtoShipperPost.Phone
            };

            unitOfWork.Add(shipper);

            unitOfWork.Commit();

            var newShipper = await unitOfWork.Query<Shipper>().LastAsync();

            return newShipper;
        }

        public async Task Delete(int id)
        {
            var shipper = await unitOfWork.Query<Shipper>().FirstOrDefaultAsync(s => s.ShipperID == id);

            if (shipper == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(shipper);
        }

        public IQueryable<Shipper> Get()
        {
            return unitOfWork.Query<Shipper>();
        }

        public async Task<DtoShipperGet> GetById(int id)
        {
            var shipper = await unitOfWork.Query<Shipper>().FirstOrDefaultAsync(s => s.ShipperID == id);

            if (shipper == null)
                throw new KeyNotFoundException();

            var shipperDto = new DtoShipperGet()
            {
                ShipperID = shipper.ShipperID,
                CompanyName = shipper.CompanyName,
                Phone = shipper.Phone,
                Orders = shipper.Orders
            };

            return shipperDto;
        }

        public async Task<Shipper> Update(int id, DtoShipperPut dtoShipperPut)
        {
            var shipper = await unitOfWork.Query<Shipper>().FirstOrDefaultAsync(s => s.ShipperID == id);

            if (shipper == null)
                throw new KeyNotFoundException();

            shipper.CompanyName = dtoShipperPut.CompanyName;
            shipper.Phone = dtoShipperPut.Phone;

            unitOfWork.Commit();

            return shipper;
        }
    }
}
