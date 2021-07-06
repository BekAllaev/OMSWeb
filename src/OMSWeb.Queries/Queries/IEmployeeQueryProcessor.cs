using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using OMSWeb.Dto.Model.EmployeeDto;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public interface IEmployeeQueryProcessor
    {
        IQueryable<Employee> Get();

        Task<Employee> GetById(int id);

        Task<Employee> Update(int id, DtoEmployeePut dtoEmployeePut);

        Task<Employee> Create(DtoEmployeePost dtoEmployeePost);

        Task Delete(int id);
    }
}
