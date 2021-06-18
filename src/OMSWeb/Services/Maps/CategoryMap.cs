using AutoMapper;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CategoryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Maps
{
    public class CategoryMap : IAutoMapperTypeConfigurator
    {
        public void Configure(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Category, DtoCategoryGet>();
        }
    }
}
