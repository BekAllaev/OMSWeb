using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Pagination
{
    public static class PaginationHelper
    {
        public static PagedResponse<IEnumerable<T>> CreatePagedReponse<T>(IEnumerable<T> pagedData, uint pageNumber, uint pageSize, int totalRecords, IUriService uriService, string route)
        {
            var respose = new PagedResponse<IEnumerable<T>>(pagedData, (int)pageNumber, (int)pageSize);
            var totalPages = ((double)totalRecords / (double)pageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            respose.NextPage =
                pageNumber >= 1 && pageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationInfo(pageNumber + 1, pageSize), route)
                : null;

            respose.PreviousPage =
                pageNumber - 1 >= 1 && pageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationInfo(pageNumber - 1, pageSize), route)
                : null;

            respose.FirstPage = uriService.GetPageUri(new PaginationInfo(1, pageSize), route);
            respose.LastPage = uriService.GetPageUri(new PaginationInfo((uint)roundedTotalPages, pageSize), route);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;

            return respose;
        }
    }
}
