using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Middleware
{
    //About middleware - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0
    //Write custom middleware - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-5.0
    public class PageSizeMiddleware
    {
        const string PAGE_SIZE_LITERAL = "PageSize";
        RequestDelegate next;
        IMemoryCache memoryCache;
        int pageSize;

        public PageSizeMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            this.next = next;
            this.memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (StringValues.IsNullOrEmpty(httpContext.Request.Query[PAGE_SIZE_LITERAL]))
            {
                if (memoryCache.TryGetValue(PAGE_SIZE_LITERAL, out pageSize))
                {
                    List<KeyValuePair<string, StringValues>> keyValuesParameters = new List<KeyValuePair<string, StringValues>>();

                    var queryCollection = httpContext.Request.Query;

                    foreach (var query in queryCollection)
                    {
                        if (query.Key == PAGE_SIZE_LITERAL)
                            keyValuesParameters.Add(new KeyValuePair<string, StringValues>(PAGE_SIZE_LITERAL, new StringValues(pageSize.ToString())));
                        else
                            keyValuesParameters.Add(new KeyValuePair<string, StringValues>(query.Key, query.Value));
                    }

                    httpContext.Request.QueryString = QueryString.Create(keyValuesParameters);
                }
            }
            else
            {
                int pageSize = Convert.ToInt16(httpContext.Request.Query[PAGE_SIZE_LITERAL]);
                memoryCache.Set(PAGE_SIZE_LITERAL, pageSize);
            }

            await next(httpContext);
        }
    }
}
