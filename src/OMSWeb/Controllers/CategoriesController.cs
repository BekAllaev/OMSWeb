using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CategoryDto;
using OMSWeb.Services.Maps;
using OMSWeb.Queries.Queries;
using OMSWeb.Services.Pagination;
using OMSWeb.Wrappers;
using OMSWeb.Queries.Interfaces;

namespace OMSWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public CategoriesController(ICategoryQueryProcessor queryProcessor, IAutoMapper autoMapper, IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var validPaginationInfo = new PaginationInfo(pageSize, pageNumber);
            var route = Request.Path.Value;

            var query = queryProcessor.Get();

            var pagedDataQuery = query
                .Skip(((int)validPaginationInfo.PageNumber - 1) * (int)validPaginationInfo.PageSize)
                .Take((int)validPaginationInfo.PageSize)
                .Include(category => category.Products);

            var pagedData = pagedDataQuery.ToList();

            var resultCollection = autoMapper.Map<List<DtoCategoryGet>>(pagedData);

            var totalRecords = query.Count();
            var pagedReponse = PaginationHelper.CreatePagedReponse<DtoCategoryGet>(resultCollection, validPaginationInfo, totalRecords, uriService, route);

            return Ok(pagedReponse);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<DtoCategoryGet> GetCategory(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoCategoryGet>(item);
            return model;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<DtoCategoryGet> PutCategory(int id, [FromBody] DtoCategoryPut dtoCategory)
        {
            var item = await queryProcessor.Update(id, dtoCategory);
            var model = autoMapper.Map<DtoCategoryGet>(item);
            return model;
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoCategoryGet> PostCategory([FromBody] DtoCategoryPost dtoCategory)
        {
            var item = await queryProcessor.Create(dtoCategory);

            var categoryModel = autoMapper.Map<DtoCategoryGet>(item);

            return categoryModel;
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task DeleteCategory(int id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
