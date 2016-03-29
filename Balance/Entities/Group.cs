using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public ICollection<int> UserIds { get; set; }
        public ICollection<Credit> Credits { get; set; }
    }
}
