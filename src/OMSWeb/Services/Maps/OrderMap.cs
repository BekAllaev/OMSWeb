using AutoMapper;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.OrderDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Maps
{
    public class OrderMap : IAutoMapperTypeConfigurator
    {
        public void Configure(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Order, DtoOrderGetWithDetails>();
            configuration.CreateMap<Order, DtoOrderGetWithoutDetails>();
        }
    }
}
