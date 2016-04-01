using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace MVCModels.Models
{
    public class PaymentModel
    {
        public string Type { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}