using OMSWeb.Services.Maps;
using OMSWeb.Services.Pagination;
using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OMSWeb.Extensions
{
    public static class ControllerExtensions
    {
        public static IQueryable<TEntity> PaginateQuery<TEntity>(this IQueryable<TEntity> query, uint pageNumber, uint pageSize)
        {
            var validPaginationInfo = new PaginationInfo(pageSize, pageNumber);

            var pagedEntities = query
                .Skip(((int)validPaginationInfo.PageNumber - 1) * (int)validPaginationInfo.PageSize)
                .Take((int)validPaginationInfo.PageSize);

            return pagedEntities;
        }

        public static PagedResponse<IEnumerable<TDto>> GetPagedResponse<TEntity, TDto>(this IEnumerable<TEntity> data, 
                                                                                       IAutoMapper autoMapper, 
                                                                                       IUriService uriService, 
                                                                                       string route, 
                                                                                       uint pageNumber, 
                                                                                       uint pageSize,
                                                                                       int count)
        {
            var resultCollection = autoMapper.Map<List<TDto>>(data);

            var pagedReponse = PaginationHelper.CreatePagedReponse<TDto>(resultCollection, pageNumber, pageSize, count, uriService, route);

            return pagedReponse;
        }
    }
}
