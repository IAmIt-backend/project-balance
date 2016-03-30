using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCModels.Models
{
    public class PaymentListItemModel
    {
        public ObjectId Id { get; set; }
        public decimal Value { get; set; }
    }
}
