using Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAbstraction
{
    public interface IGroupRepository
    {
        Task AddGroup(Group group);
        Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId);
        Task AddPayment(ObjectId groupId, Payment payment);
        Task<ICollection<ObjectId>> GetAllUsersInGroup(ObjectId groupId);
        Task<Group> GetGroup(ObjectId id);
        Task<ICollection<Payment>> GetAllPayments(ObjectId groupId);
        Task<Payment> GetPayment(ObjectId groupId, ObjectId userId);
        Task<bool> IsGroupActive(ObjectId groupId);
        Task SetGroupState(ObjectId groupId, State state);
    }
}
