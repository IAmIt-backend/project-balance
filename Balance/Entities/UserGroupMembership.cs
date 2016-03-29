using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum Role
    {
        Member,
        Administrator
    }

    public class UserGroupMembership
    {
        public ObjectId GroupId { get; set; }
        public ObjectId UserId { get; set; }
        public  Role MemberType { get; set; }
        
    }
}
