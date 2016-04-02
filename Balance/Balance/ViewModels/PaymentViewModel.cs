using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Balance.ViewModels
{
    public class PaymentViewModel
    {
        public List<string> Types { get; set; }
        public decimal Value { get; set; }
    }
}