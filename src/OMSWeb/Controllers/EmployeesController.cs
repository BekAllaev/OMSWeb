using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.EmployeeDto;
using OMSWeb.Extensions;
using OMSWeb.Queries.Interfaces;
using OMSWeb.Queries.Queries;
using OMSWeb.Services.Maps;
using OMSWeb.Services.Pagination;
using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public EmployeesController(IEmployeeQueryProcessor queryProcessor, IAutoMapper autoMapper, IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var pagedData = queryProcessor.Get().PaginateQuery(pageNumber, pageSize).ToList();

            var pagedResponse = pagedData.GetPagedResponse<Employee, DtoEmployeeGet>(autoMapper, uriService, Request.Path.Value, pageNumber, pageSize, queryProcessor.Get().Count());

            return Ok(pagedResponse);
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<DtoEmployeeGet> GetEmployee(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoEmployeeGet>(item); 
            return model;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<DtoEmployeeGet> PutEmployee(int id, [FromBody] DtoEmployeePut dtoEmployee)
        {
            var item = await queryProcessor.Update(id, dtoEmployee);
            var model = autoMapper.Map<DtoEmployeeGet>(item);
            return model;
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoEmployeeGet> PostEmployee([FromBody] DtoEmployeePost dtoEmployee)
        {
            var item = await queryProcessor.Create(dtoEmployee);

            var product = autoMapper.Map<DtoEmployeeGet>(item);

            return product;
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task DeleteEmployee(int id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
