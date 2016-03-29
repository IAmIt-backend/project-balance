using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Payment
    {
        public ObjectId UserId { get; set; }
        public decimal Value { get; set; }
    }
}
