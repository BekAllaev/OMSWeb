using Microsoft.AspNetCore.WebUtilities;
using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Pagination
{
    public class UriService : IUriService
    {
        private readonly string baseUri;
        public UriService(string baseUri)
        {
            this.baseUri = baseUri;
        }

        public Uri GetPageUri(PaginationInfo paginationInfo, string route)
        {
            var _enpointUri = new Uri(string.Concat(baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", paginationInfo.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", paginationInfo.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
