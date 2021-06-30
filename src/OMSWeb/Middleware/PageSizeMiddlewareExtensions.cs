using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Middleware
{
    public static class PageSizeMiddlewareExtensions
    {
        public static IApplicationBuilder UsePageSizeCaching(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PageSizeMiddleware>();
        }
    }
}
