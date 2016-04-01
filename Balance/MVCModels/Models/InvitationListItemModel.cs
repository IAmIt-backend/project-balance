using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCModels.Models
{
    public class InvitationListItemModel
    {
        public ObjectId GroupId { get; set; }
        public bool IsVerified { get; set; }
        public bool IsRejected { get; set; }
    }
}
