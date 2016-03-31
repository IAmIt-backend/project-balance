using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCModels.Models;

namespace Balance.ViewModels
{
    public class CountViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public List<CreditModel> Credits { get; set; }
    }
}