using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.EmployeeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Queries
{
    public class EmployeeQueryProcessor : IEmployeeQueryProcessor
    {
        IUnitOfWork unitOfWork;

        public EmployeeQueryProcessor(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Employee> Create(DtoEmployeePost dtoEmployeePost)
        {
            var employee = new Employee()
            {
                FirstName = dtoEmployeePost.FirstName,
                LastName = dtoEmployeePost.LastName,
                BirthDate = dtoEmployeePost.BirthDate,
                HireDate = dtoEmployeePost.HireDate,
                Title = dtoEmployeePost.Title,
                TitleOfCourtesy = dtoEmployeePost.TitleOfCourtesy,
                Address = dtoEmployeePost.Address,
                Country = dtoEmployeePost.Country,
                City = dtoEmployeePost.Country,
                PostalCode = dtoEmployeePost.PostalCode,
                HomePhone = dtoEmployeePost.HomePhone,
                Extension = dtoEmployeePost.Extension,
                Photo = dtoEmployeePost.Photo,
                PhotoPath = dtoEmployeePost.PhotoPath,
                Region = dtoEmployeePost.Region,
                Notes = dtoEmployeePost.Notes,
            };

            unitOfWork.Add(employee);

            unitOfWork.Commit();

            var newEmployee = await unitOfWork.Query<Employee>().LastAsync();

            return newEmployee;
        }

        public async Task Delete(int id)
        {
            var employee = await unitOfWork.Query<Employee>().FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(employee);

            unitOfWork.Commit();
        }

        public IQueryable<Employee> Get()
        {
            return unitOfWork.Query<Employee>();
        }

        public async Task<DtoEmployeeGet> GetById(int id)
        {
            var employee = await unitOfWork.Query<Employee>().FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                throw new KeyNotFoundException();

            var employeeDto = new DtoEmployeeGet()
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Address = employee.Address,
                Photo = employee.Photo,
                PhotoPath = employee.PhotoPath,
                BirthDate = employee.BirthDate,
                City = employee.City,
                Country = employee.Country,
                Extension = employee.Extension,
                HireDate = employee.HireDate,
                HomePhone = employee.HomePhone,
                Notes = employee.Notes,
                TitleOfCourtesy = employee.TitleOfCourtesy,
                Title = employee.Title,
                Region = employee.Region,
                Orders = employee.Orders,
                PostalCode = employee.PostalCode,
                ReportsTo = employee.ReportsTo
            };

            return employeeDto;
        }

        public async Task<Employee> Update(int id, DtoEmployeePut dtoEmployeePut)
        {
            var employee = await unitOfWork.Query<Employee>().FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                throw new KeyNotFoundException();

            employee.Title = dtoEmployeePut.Title;
            employee.TitleOfCourtesy = dtoEmployeePut.TitleOfCourtesy;
            employee.Photo = dtoEmployeePut.Photo;
            employee.PhotoPath = dtoEmployeePut.PhotoPath;
            employee.Country = dtoEmployeePut.Country;
            employee.City = dtoEmployeePut.City;
            employee.Address = dtoEmployeePut.Address;
            employee.Notes = dtoEmployeePut.Notes;
            employee.HomePhone = dtoEmployeePut.HomePhone;
            employee.ReportsTo = dtoEmployeePut.ReportsTo;
            employee.Region = dtoEmployeePut.Region;
            employee.PostalCode = dtoEmployeePut.PostalCode;
            employee.Extension = dtoEmployeePut.Extension;

            unitOfWork.Update(employee);

            unitOfWork.Commit();

            var updatedEmployee = await unitOfWork.Query<Employee>().FirstAsync(e => e.EmployeeID == id);

            return updatedEmployee;
        }
    }
}
