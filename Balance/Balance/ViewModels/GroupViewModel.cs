using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MVCModels.Models;

namespace Balance.ViewModels
{
    public class GroupViewModel
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<PaymentListItemModel> Payments { get; set; }
        public decimal Sum { get; set; }
    }
}