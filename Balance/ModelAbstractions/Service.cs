using Entities;
using MongoDB;
using MongoDB.Bson;
using MVCModels.Models;
using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelAbstractions
{
    public class Service : IService
    {


        private IGroupRepository _groups = new DbGroupRepository();
        private IUserRepository _users = new DbUserRepository();


        public async Task AddGroup(AddGroupModel groupModel, ObjectId userId)
        {
            var groups = await _users.GetAllGroupsOfUser(userId);
            var id = ObjectId.GenerateNewId();
            if (groups.Select(g => userId).Contains(id))
            {
                throw new Exception("Такая группа уже существует");
            }
            var group = new Group
            {
                Id = id,
                Name = groupModel.Name,
                Description = groupModel.Description,
                Payments = new List<Payment>(),
                State = State.Active
            };
            await _groups.AddGroup(group);
            await _groups.AddUserToGroup(Role.Administrator, userId, id);

        }



        public async Task AddUserToGroup(ObjectId userId, ObjectId groupId)
        {
            if (await _users.IsUserInGroup(userId, groupId) == true)
            {
                throw new Exception("Этот пользователь уже находится в группе");
            }
            else
            {
                await _groups.AddUserToGroup(Role.Member, userId, groupId);
            }

        }



        public async Task<AddGroupModel> GetGroup(ObjectId id)
        {
            var group = await _groups.GetGroup(id);
            if (group == null)
            {
                throw new Exception("Такой группы не существует");
            }

            return new AddGroupModel { Name = group.Name, Description = group.Description };
        }

        public async Task AddPayment(ObjectId groupId, decimal value, ObjectId userId)
        {
            var group = await _groups.GetGroup(groupId);
            await _groups.AddPayment(groupId, new Payment { UserId = userId, Value = value });
        }


        public async Task<ICollection<PaymentListItemModel>> GetAllPayments(ObjectId groupId)
        {
            var group = await _groups.GetGroup(groupId);
            var payments = await _groups.GetAllPayments(groupId);
            if (group == null)
            {
                throw new Exception("Такой группы не существует");
            }
            return payments.Select(p => new PaymentListItemModel { Id = p.UserId, Value = p.Value }).ToList();

        }

        public async Task<ICollection<ObjectId>> GetAllUsersInGroup(ObjectId groupId)
        {
            var users = await _groups.GetAllUsersInGroup(groupId);
            return users.ToList(); 
        }


        public async Task<ICollection<GroupListItemModel>> GetAllGroupsOfUser(ObjectId userId)
        {
            var groups = (await _users.GetAllGroupsOfUser(userId));
            return groups.Select(g => new GroupListItemModel { Id = g.Id, Name = g.Name }).ToList();
        }


        public async Task<bool> IsUserAdministrator(ObjectId userId, ObjectId groupId)
        {
            var role = await _users.IsUserAdministrator(userId, groupId);
            return role;
        }

        public async Task VerifyInvitation(ObjectId userId, ObjectId groupId)
        {
            await _users.VerifyInvitation(userId, groupId);
        }

        public async Task RejectInvitation(ObjectId userId, ObjectId groupId)
        {
            await _users.RejectInvitation(userId, groupId);
        }

        public async Task<ICollection<AddGroupModel>> GetAllInvitations(ObjectId userId)
        {
           return (await _users.GetAllInvitations(userId))
                .Select(g => new AddGroupModel {Name = g.Name, Description = g.Description }).ToList();
        }

        public async Task<bool> IsGroupActive(ObjectId groupId)
        {
            return await _groups.IsGroupActive(groupId); 
        }

        public async Task SetGroupState(ObjectId groupId)
        {
            await _groups.SetGroupState(groupId, State.Passive);
        }
    }
}
