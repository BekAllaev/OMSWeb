using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ShipperDto;
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
    public class ShippersController : ControllerBase
    {
        private readonly IShipperQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public ShippersController(IShipperQueryProcessor queryProcessor, IAutoMapper autoMapper, IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Shippers
        [HttpGet]
        public async Task<IActionResult> GetShippers([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var pagedData = queryProcessor.Get().PaginateQuery(pageNumber, pageSize).ToList();

            var pagedResponse = pagedData.GetPagedResponse<Shipper, DtoShipperGet>(autoMapper, uriService, Request.Path.Value, pageNumber, pageSize, queryProcessor.Get().Count());

            return Ok(pagedResponse);
        }

        // GET: api/Shippers/5
        [HttpGet("{id}")]
        public async Task<DtoShipperGet> GetShipper(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoShipperGet>(item);
            return model;
        }

        // PUT: api/Shippers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<DtoShipperGet> PutShipper(int id, [FromBody] DtoShipperPut dtoShipper)
        {
            var item = await queryProcessor.Update(id, dtoShipper);
            var model = autoMapper.Map<DtoShipperGet>(item);
            return model;
        }

        // POST: api/Shippers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoShipperGet> PostShipper([FromBody] DtoShipperPost dtoShipper)
        {
            var item = await queryProcessor.Create(dtoShipper);

            var product = autoMapper.Map<DtoShipperGet>(item);

            return product;
        }

        // DELETE: api/Shipper/5
        [HttpDelete("{id}")]
        public async Task DeleteShipper(int id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
