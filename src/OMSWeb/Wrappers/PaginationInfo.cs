using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Wrappers
{
    public class PaginationInfo
    {
        public uint PageNumber { get; set; }

        public uint PageSize { get; set; }

        public PaginationInfo(uint pageSize = 10, uint pageNumber = 1)
        {
            this.PageNumber = pageNumber; 
            this.PageSize = pageSize; 
        }
    }
}
