using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Dto.Model.CategoryDto
{
    public class DtoCategoryPut
    {
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
