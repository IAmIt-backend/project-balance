using Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAbstraction
{
    public interface IUserRepository
    {
        Task<ICollection<Group>> GetAllGroupsOfUser(ObjectId userId);
        Task<bool> IsUserAdministrator(ObjectId groupId, ObjectId userId);
        Task<bool> IsUserInGroup(ObjectId groupId, ObjectId userId);
        Task<bool> IsUserInvitedInGroup(ObjectId groupId, ObjectId userId);
        Task VerifyInvitation(ObjectId groupId, ObjectId userId);
        Task RejectInvitation(ObjectId groupId, ObjectId userId);
        Task<ICollection<Group>> GetAllInvitations(ObjectId userId);
    }
}
