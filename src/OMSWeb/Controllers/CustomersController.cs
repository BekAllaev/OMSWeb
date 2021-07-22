using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CutomerDto;
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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public CustomersController(ICustomerQueryProcessor queryProcessor, IAutoMapper autoMapper, IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var pagedData = queryProcessor.Get().PaginateQuery(pageNumber, pageSize).ToList();

            var pagedResponse = pagedData.GetPagedResponse<Customer, DtoCustomerGet>(autoMapper, uriService, Request.Path.Value, pageNumber, pageSize, queryProcessor.Get().Count());

            return Ok(pagedResponse);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<DtoCustomerGet> GetCustomer(string id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoCustomerGet>(item);
            return model;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<DtoCustomerGet> PutCustomer(string id, [FromBody] DtoCustomerPutPost dtoCustomer)
        {
            var item = await queryProcessor.Update(id, dtoCustomer);
            var model = autoMapper.Map<DtoCustomerGet>(item);
            return model;
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoCustomerGet> PostCustomer([FromBody] DtoCustomerPutPost dtoCustomer)
        {
            var item = await queryProcessor.Create(dtoCustomer);

            var categoryModel = autoMapper.Map<DtoCustomerGet>(item);

            return categoryModel;
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task DeleteCustomer(string id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
