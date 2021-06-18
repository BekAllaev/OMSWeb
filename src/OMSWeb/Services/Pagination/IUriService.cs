using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Pagination
{
    public interface IUriService
    {
        /// <summary>
        /// Generates uri with pagination parameters
        /// </summary>
        /// <param name="paginationInfo"></param>
        /// <param name="route">
        /// Controller name
        /// </param>
        /// <returns></returns>
        Uri GetPageUri(PaginationInfo paginationInfo, string route);
    }
}
