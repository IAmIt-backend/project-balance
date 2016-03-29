using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelAbstractions
{
    public interface IService
    {
       void AddGroup(ObjectId groupId);
       void AddUserToGroup(ObjectId userId, ObjectId groupId);
       void GetAllGroups();
    }
}
