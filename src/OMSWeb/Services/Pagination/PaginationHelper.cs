using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Pagination
{
    public static class PaginationHelper
    {
        public static PagedResponse<IEnumerable<T>> CreatePagedReponse<T>(IEnumerable<T> pagedData, PaginationInfo validFilter, int totalRecords, IUriService uriService, string route)
        {
            var respose = new PagedResponse<IEnumerable<T>>(pagedData, (int)validFilter.PageNumber, (int)validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            respose.NextPage =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationInfo(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null;

            respose.PreviousPage =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationInfo(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null;

            respose.FirstPage = uriService.GetPageUri(new PaginationInfo(1, validFilter.PageSize), route);
            respose.LastPage = uriService.GetPageUri(new PaginationInfo((uint)roundedTotalPages, validFilter.PageSize), route);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;

            return respose;
        }
    }
}
