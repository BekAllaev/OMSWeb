using Hangfire;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Queries.Caching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Extensions
{
    public static class CacheExtensions
    {
        public static IQueryable<T> GetCacheOrQuery<T>(this ICacheService cacheService, IUnitOfWork unitOfWork, string cacheKey) where T : class
        {
            if (!cacheService.TryGet(cacheKey, out IEnumerable<T> list))
            {
                var query = unitOfWork.Query<T>();

                list = query.ToList();
                cacheService.Set(cacheKey, list);
                return query;
            }
            else
                return list.AsQueryable();
        }

        public static async Task RefreshCacheAsync<T>(this ICacheService cacheService, IUnitOfWork unitOfWork, string cacheKey) where T : class
        {
            cacheService.Remove(cacheKey);
            var list = await unitOfWork.Query<T>().ToListAsync();
            cacheService.Set(cacheKey, list);
        }
    }
}
