using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMSWeb.Data.Model;

namespace OMSWeb.Dto.Model.EmployeeDto
{
    public class DtoEmployeeGetBuisnessCard
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string HomePhone { get; set; }
        public string Extension { get; set; }
        public string Notes { get; set; }
    }
}
