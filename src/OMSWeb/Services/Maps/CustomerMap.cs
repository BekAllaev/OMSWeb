using AutoMapper;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.CutomerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Maps
{
    public class CustomerMap : IAutoMapperTypeConfigurator
    {
        public void Configure(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Customer, DtoCustomerGet>();
        }
    }
}
