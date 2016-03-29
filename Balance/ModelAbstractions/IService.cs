using Balance.Models;
using Entities;
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
        Task<User> GetUser(string email);
        Task AddPayment(ObjectId groupId, double value, ObjectId userId);
        Task AddGroup(AddGroupModel groupModel);
        Task AddUserToGroup(ObjectId userId, ObjectId groupId);
        Task<GroupListItemModel> GetAllGroups();
        Task<AddGroupModel> GetGroup(ObjectId id);
    }
}
