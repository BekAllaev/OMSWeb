using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.EmployeeDto;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Interfaces;
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
        Func<CacheTech, ICacheService> cacheService;
        readonly string cacheKey = $"{typeof(Employee)}";
        readonly CacheTech cacheTech = CacheTech.Memory;

        public EmployeeQueryProcessor(IUnitOfWork unitOfWork,Func<CacheTech,ICacheService> cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
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

            var newEmployee = await unitOfWork.Query<Employee>().OrderBy(x => x.EmployeeID).LastAsync();

            BackgroundJob.Enqueue(() => RefreshCache());

            return newEmployee;
        }

        public async Task Delete(int id)
        {
            var employee = await unitOfWork.Query<Employee>().FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                throw new KeyNotFoundException();

            unitOfWork.Delete(employee);
            unitOfWork.Commit();

            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public IQueryable<Employee> Get()
        {
            if (!cacheService(cacheTech).TryGet(cacheKey, out IQueryable<Employee> cachedList))
            {
                cachedList = unitOfWork.Query<Employee>();
                cacheService(cacheTech).Set(cacheKey, cachedList.ToList());
            }

            return cachedList;
        }

        public async Task<Employee> GetById(int id)
        {
            var employee = await unitOfWork.Query<Employee>().FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                throw new KeyNotFoundException();

            return employee;
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

            BackgroundJob.Enqueue(() => RefreshCache());

            return employee;
        }

        public async Task RefreshCache()
        {
            cacheService(cacheTech).Remove(cacheKey);
            var list = await unitOfWork.Query<Employee>().ToListAsync();
            cacheService(cacheTech).Set(cacheKey, list);
        }
    }
}
