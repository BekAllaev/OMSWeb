﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ProductDto;
using OMSWeb.Services.Maps;
using OMSWeb.Queries.Queries;
using OMSWeb.Services.Pagination;
using OMSWeb.Wrappers;
using OMSWeb.Queries.Interfaces;
using OMSWeb.Extensions;

namespace OMSWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public ProductsController(IProductQueryProcessor queryProcessor,IAutoMapper autoMapper,IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var pagedData = queryProcessor.Get().PaginateQuery(pageNumber, pageSize).ToList();

            var pagedResponse = pagedData.GetPagedResponse<Product, DtoProductGet>(autoMapper, uriService, Request.Path.Value, pageNumber, pageSize, queryProcessor.Get().Count());

            return Ok(pagedResponse);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<DtoProductGet> GetProduct(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoProductGet>(item);
            return model;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<DtoProductGet> PutProduct(int id, [FromBody] DtoProductPut dtoProduct)
        {
            var item = await queryProcessor.Update(id, dtoProduct);
            var model = autoMapper.Map<DtoProductGet>(item);
            return model;
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoProductGet> PostProduct([FromBody] DtoProductPost dtoProduct)
        {
            var item = await queryProcessor.Create(dtoProduct);

            var product = autoMapper.Map<DtoProductGet>(item);

            return product;
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task DeleteProduct(int id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
