using AutoMapper;
using OMSWeb.Data.Model;
using OMSWeb.Dto.Model.EmployeeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Services.Maps
{
    public class EmployeeMap : IAutoMapperTypeConfigurator
    {
        public void Configure(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Employee, DtoEmployeeGet>();
        }
    }
}
