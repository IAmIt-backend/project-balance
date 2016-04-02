using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCModels.Models
{
    public class InvitationItemModel
    {
        public string GroupName{ get; set; }
        public ObjectId GroupId { get; set; }
        //public string AdminName { get; set; }
    }
}
