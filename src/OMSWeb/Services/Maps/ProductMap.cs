using AutoMapper;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.ProductDto;
using OMSWeb.Services.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Maps
{
    public class ProductMap : IAutoMapperTypeConfigurator
    {
        public void Configure(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Product, DtoProductGet>();
        }
    }
}
