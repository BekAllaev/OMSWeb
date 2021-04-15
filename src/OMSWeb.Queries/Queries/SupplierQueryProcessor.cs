using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.SupplierDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class SupplierQueryProcessor : ISupplierQueryProcessor
    {
        IUnitOfWork unitOfWork;

        public SupplierQueryProcessor(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Supplier> Create(DtoSupplierPost dtoSupplierPost)
        {
            var supplier = new Supplier()
            {
                CompanyName = dtoSupplierPost.CompanyName,
                ContactName = dtoSupplierPost.ContactName,
                ContactTitle = dtoSupplierPost.ContactTitle,
                Address = dtoSupplierPost.Address,
                City = dtoSupplierPost.City,
                Region = dtoSupplierPost.Region,
                PostalCode = dtoSupplierPost.PostalCode,
                Country = dtoSupplierPost.Country,
                Phone = dtoSupplierPost.Phone,
                Fax = dtoSupplierPost.Fax,
                HomePage = dtoSupplierPost.HomePage,
            };

            unitOfWork.Add(supplier);

            unitOfWork.Commit();

            var newSupplier = await unitOfWork.Query<Supplier>().LastAsync();

            return newSupplier;
        }

        public async Task Delete(int id)
        {
            var supplier = await unitOfWork.Query<Supplier>().FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(supplier);

            unitOfWork.Commit();
        }

        public IQueryable<Supplier> Get()
        {
            return unitOfWork.Query<Supplier>();
        }

        public async Task<DtoSupplierGet> GetById(int id)
        {
            var supplier = await unitOfWork.Query<Supplier>().FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
                throw new KeyNotFoundException();

            var supplierDto = new DtoSupplierGet()
            {
                SupplierID = supplier.SupplierID,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                ContactTitle = supplier.ContactTitle,
                Address = supplier.Address,
                City = supplier.City,
                Region = supplier.Region,
                PostalCode = supplier.PostalCode,
                Country = supplier.Country,
                Phone = supplier.Phone,
                Fax = supplier.Fax,
                HomePage = supplier.HomePage,
                Products = supplier.Products
            };

            return supplierDto;
        }

        public async Task<Supplier> Update(int id, DtoSupplierPut dtoSupplierPut)
        {
            var supplier = await unitOfWork.Query<Supplier>().FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
                throw new KeyNotFoundException();

            supplier.CompanyName = dtoSupplierPut.CompanyName;
            supplier.ContactName = dtoSupplierPut.ContactName;
            supplier.ContactTitle = dtoSupplierPut.ContactTitle;
            supplier.Address = dtoSupplierPut.Address;
            supplier.City = dtoSupplierPut.City;
            supplier.Region = dtoSupplierPut.Region;
            supplier.PostalCode = dtoSupplierPut.PostalCode;
            supplier.Country = dtoSupplierPut.Country;
            supplier.Phone = dtoSupplierPut.Phone;
            supplier.Fax = dtoSupplierPut.Fax;
            supplier.HomePage = dtoSupplierPut.HomePage;

            unitOfWork.Update(supplier);

            unitOfWork.Commit();

            return supplier;
        }
    }
}
