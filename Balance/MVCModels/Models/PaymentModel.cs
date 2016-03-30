using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCModels.Models
{
    public class PaymentModel
    {
        public decimal Value { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}