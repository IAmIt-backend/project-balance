using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MVCModels.Models
{
    public class CountListItemModel
    {
        public ObjectId Id { get; set; }
        public decimal Value { get; set; }
    }
}
