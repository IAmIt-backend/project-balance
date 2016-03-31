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
        Task AddPayment(ObjectId groupId, decimal value, ObjectId userId);
        Task<ICollection<PaymentListItemModel>> GetAllPayments(ObjectId groupId);

        Task AddGroup(AddGroupModel groupModel, ObjectId userId);
        Task AddUserToGroup(ObjectId userId, ObjectId groupId);
        Task<ICollection<ObjectId>> GetAllUsersInGroup(ObjectId groupId);
        Task<AddGroupModel> GetGroup(ObjectId id);

        Task<ICollection<GroupListItemModel>> GetAllGroupsOfUser(ObjectId userId);
        Task<bool> IsUserAdministrator(ObjectId userId, ObjectId groupId);

        Task VerifyInvitation(ObjectId userId, ObjectId groupId);
        Task RejectInvitation(ObjectId userId, ObjectId groupId);
        Task<ICollection<AddGroupModel>> GetAllInvitations(ObjectId userId);

        Task<bool> IsGroupActive(ObjectId groupId);
        Task SetGroupState(ObjectId groupId);
    }
}