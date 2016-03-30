using Entities;
using MongoDB.Bson;
using MVCModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelAbstractions
{
    public interface IService
    {
        //Task<User> GetUser(string email);
        Task AddPayment(ObjectId groupId, decimal value, ObjectId userId);
        Task<ICollection<PaymentListItemModel>> GetAllPayments(ObjectId groupId);
        Task AddGroup(AddGroupModel groupModel);
        Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId);
        Task<ICollection<GroupListItemModel>> GetAllGroups();
        Task<AddGroupModel> GetGroup(ObjectId id);
    }
}
