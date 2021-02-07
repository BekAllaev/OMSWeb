using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.API.Models.Orders
{
    public class UpdateOrderDTO
    {
        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        [StringLength(50)]
        public string ShipName { get; set; }

        [StringLength(50)]
        public string ShipAddress { get; set; }

        [StringLength(50)]
        public string ShipCity { get; set; }

        [StringLength(50)]
        public string ShipRegion { get; set; }

        [StringLength(50)]
        public string ShipPostalCode { get; set; }

        [StringLength(50)]
        public string ShipCountry { get; set; }

        public IList<Order_Detail> Order_Details { get; set; }
    }
}
