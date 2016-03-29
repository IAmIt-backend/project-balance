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
        public ObjectId userId { get; set; }
        public double Value { get; set; }
    }
}
