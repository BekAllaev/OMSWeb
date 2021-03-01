﻿using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Dto.Model.ShipperDto
{
    public class DtoShipperGet
    {
        public int ShipperID { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
