using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Queries.Caching.Services
{
    /// <summary>
    /// Cache data, get cached data and clear cached data
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Get cached data
        /// </summary>
        /// <typeparam name="T">
        /// Type of cached data
        /// </typeparam>
        /// <param name="cacheKey">
        /// Key by which data is cached
        /// </param>
        /// <param name="value">
        /// Where cached data will be placed
        /// </param>
        /// <returns>
        /// True if data exist in cache
        /// False if data is not exist in cache
        /// </returns>
        bool TryGet<T>(string cacheKey, out T value);

        /// <summary>
        /// Cache data
        /// </summary>
        /// <typeparam name="T">
        /// Type of cached data
        /// </typeparam>
        /// <param name="cacheKey">
        /// Key by which data is cached
        /// </param>
        /// <param name="value">
        /// Data that must to be cached
        /// </param>
        /// <returns>
        /// Returns data that was cached and then was gotten from cache
        /// </returns>
        T Set<T>(string cacheKey, T value);

        /// <summary>
        /// Remove cached data by key
        /// </summary>
        /// <param name="cacheKey">
        /// Key of cached data that must be removed
        /// </param>
        void Remove(string cacheKey);
    }
}
