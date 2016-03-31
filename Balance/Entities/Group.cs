using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum State { Active, Passive }
    public class Group
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public State State { get; set; }
        public string Description { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
