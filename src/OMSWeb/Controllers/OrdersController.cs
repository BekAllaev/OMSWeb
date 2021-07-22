using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.OrderDto;
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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public OrdersController(IOrderQueryProcessor queryProcessor, IAutoMapper autoMapper, IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var pagedData = queryProcessor.Get().PaginateQuery(pageNumber, pageSize).Include(order => order.Order_Details).ToList();

            var pagedResponse = pagedData.GetPagedResponse<Order, DtoOrderGetWithDetails>(autoMapper, uriService, Request.Path.Value, pageNumber, pageSize, queryProcessor.Get().Count());

            return Ok(pagedResponse);
        }

        // GET: api/GetOrderWithDetails/5
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<DtoOrderGetWithDetails> GetOrderWithDetails(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoOrderGetWithDetails>(item);
            return model;
        }

        // GET: api/GetOrderWithoutDetails/5
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<DtoOrderGetWithoutDetails> GetOrderWithoutDetails(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoOrderGetWithoutDetails>(item);
            return model;
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoOrderGetWithDetails> PostOrder([FromBody] DtoOrderPost dtoOrder)
        {
            var item = await queryProcessor.Create(dtoOrder);

            var order = autoMapper.Map<DtoOrderGetWithDetails>(item);

            return order;
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task DeleteOrder(int id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
