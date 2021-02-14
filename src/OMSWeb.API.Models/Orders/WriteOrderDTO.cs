using OMSWeb.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMSWeb.API.Models.Orders
{
    public class WriteOrderDTO
    {
        [Required]
        public string CustomerID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public DateTime? OrderDate { get; set; }

        [Required]
        public int? ShipVia { get; set; }

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

        [Required]
        public IList<Order_Detail> OrderDetails { get; set; }
    }
}
