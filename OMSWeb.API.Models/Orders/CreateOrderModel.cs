﻿using System;
using System.ComponentModel.DataAnnotations;

namespace OMSWeb.API.Models.Orders
{
    public class CreateOrderModel
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0.01, int.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
