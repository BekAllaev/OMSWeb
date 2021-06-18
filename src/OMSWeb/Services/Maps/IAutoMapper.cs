using AutoMapper; // NuGet - AutoMapper
using System.Collections.Generic;
using System.Linq;

namespace OMSWeb.Maps
{
    /// <summary>
    /// Wrapper above <see cref="IMapper"/> 
    /// Motivation is to hide some of <see cref="IMapper"/> methods
    /// </summary>
    public interface IAutoMapper
    {
        IConfigurationProvider Configuration { get; }

        T Map<T>(object objectToMap);

        void Map<TSource, TDestination>(TSource source, TDestination destination);

        TResult[] Map<TSource, TResult>(IEnumerable<TSource> sourceQuery);

        IQueryable<TResult> Map<TSource, TResult>(IQueryable<TSource> sourceQuery);
    }
}
