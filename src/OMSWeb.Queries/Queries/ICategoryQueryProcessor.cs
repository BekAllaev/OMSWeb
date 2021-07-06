using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CategoryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public interface ICategoryQueryProcessor
    {
        IQueryable<Category> Get();

        Task<Category> GetById(int id);

        Task<Category> Update(int id, DtoCategoryPut dtoCategoryPut);

        Task<Category> Create(DtoCategoryPost dtoCategoryPost);

        Task Delete(int id);
    }
}
