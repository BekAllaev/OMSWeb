using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ProductDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class ProductQueryProcessor : IProductQueryProcessor
    {
        IUnitOfWork unitOfWork;

        public ProductQueryProcessor(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Product> Create(DtoProductPost dtoProductPost)
        {
            var product = new Product()
            {
                CategoryID = dtoProductPost.CategoryID,
                SupplierID = dtoProductPost.SupplierID,
                ProductName = dtoProductPost.ProductName,
                UnitPrice = dtoProductPost.UnitPrice,
                QuantityPerUnit = dtoProductPost.QuantityPerUnit,
                Discontinued = dtoProductPost.Discontinued,
                UnitsInStock = dtoProductPost.UnitsInStock,
                UnitsOnOrder = dtoProductPost.UnitsOnOrder,
                ReorderLevel = dtoProductPost.ReorderLevel
            };

            unitOfWork.Add(product);

            unitOfWork.Commit();

            var newProduct = await unitOfWork.Query<Product>().LastAsync();

            return newProduct;
        }

        public async  Task Delete(int id)
        {
            var product = await unitOfWork.Query<Product>().FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(product);

            unitOfWork.Commit();
        }

        public IQueryable<Product> Get()
        {
            return unitOfWork.Query<Product>();
        }

        public async Task<DtoProductGet> GetById(int id)
        {
            var product = await unitOfWork.Query<Product>().FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new KeyNotFoundException();

            var productDto = new DtoProductGet()
            {
                ProductID = product.ProductID,
                CategoryID = product.CategoryID,
                SupplierID = product.SupplierID,
                ProductName = product.ProductName,
                Discontinued = product.Discontinued,
                QuantityPerUnit = product.QuantityPerUnit,
                UnitPrice = product.UnitPrice,
                ReorderLevel = product.ReorderLevel,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder
            };

            return productDto;
        }

        public async Task<Product> Update(int id, DtoProductPut dtoProductPut)
        {
            var product = await unitOfWork.Query<Product>().FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new KeyNotFoundException();

            product.SupplierID = dtoProductPut.SupplierID;
            product.QuantityPerUnit = dtoProductPut.QuantityPerUnit;
            product.UnitPrice = dtoProductPut.UnitPrice;
            product.UnitsInStock = dtoProductPut.UnitsInStock;
            product.UnitsOnOrder = dtoProductPut.UnitsOnOrder;
            product.ReorderLevel = dtoProductPut.ReorderLevel;
            product.Discontinued = dtoProductPut.Discontinued;

            unitOfWork.Update(product);

            unitOfWork.Commit();

            var updatedProduct = await unitOfWork.Query<Product>().FirstAsync(p => p.ProductID == id);

            return updatedProduct;
        }
    }
}
