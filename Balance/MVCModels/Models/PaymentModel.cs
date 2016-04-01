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
        [Required]
        public decimal Value { get; set; }
    }
}