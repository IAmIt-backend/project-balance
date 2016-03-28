using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Credit
    {
        public Guid Id { get; set; }
        public int Money { get; set; }
        //public int DebtorId { get; set; }
        //public int CreditorId { get; set; }
    }
}
