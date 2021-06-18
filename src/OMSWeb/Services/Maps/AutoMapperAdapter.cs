using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Maps
{
    public class AutoMapperAdapter : IAutoMapper
    {
        private readonly IMapper mapper;

        public AutoMapperAdapter(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IConfigurationProvider Configuration => mapper.ConfigurationProvider;

        public T Map<T>(object objectToMap)
        {
            return mapper.Map<T>(objectToMap);
        }
 
        public TResult[] Map<TSource, TResult>(IEnumerable<TSource> sourceQuery)
        {
            return sourceQuery.Select(x => mapper.Map<TResult>(x)).ToArray();
        }
 
        public IQueryable<TResult> Map<TSource, TResult>(IQueryable<TSource> sourceQuery)
        {
            return sourceQuery.ProjectTo<TResult>(mapper.ConfigurationProvider);
        }
 
        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            mapper.Map(source, destination);
        }    
    }
}
