using Entities;
using MongoDB;
using MongoDB.Bson;
using MVCModels.Models;
using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Converter;

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
            await _groups.AddUserToGroup(Role.Administrator, id, userId);
            await _groups.AddPayment(id, new Payment {Value = 0, UserId = userId, CurrencyType = CurrencyType.USD});

        }



        public async Task AddUserToGroup(ObjectId userId, ObjectId groupId)
        {
            if (await _users.IsUserInGroup(groupId, userId) || await _users.IsUserInvitedInGroup(groupId,userId))
            {
                throw new Exception("Этот пользователь уже находится в группе");
            }
            else
            {
                await _groups.AddUserToGroup(Role.Member, groupId, userId);
                //await _groups.AddPayment(groupId, new Payment { Value = 0, UserId = userId });
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

        public async Task AddPayment(ObjectId groupId, decimal value, ObjectId userId, string type)
        {
            var group = await _groups.GetGroup(groupId);
            await _groups.AddPayment(groupId, new Payment { UserId = userId, Value = value, CurrencyType = type});
        }


        public async Task<ICollection<PaymentListItemModel>> GetAllPayments(ObjectId groupId)
        {
            var group = await _groups.GetGroup(groupId);
            var payments = await _groups.GetAllPayments(groupId);
            if (group == null)
            {
                throw new Exception("Такой группы не существует");
            }
            return payments.Select(p => new PaymentListItemModel { Id = p.UserId, Value = p.Value, Type = p.CurrencyType }).ToList();

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
            var role = await _users.IsUserAdministrator(groupId,userId);
            return role;
        }

        public async Task VerifyInvitation(ObjectId userId, ObjectId groupId)
        {
            await _users.VerifyInvitation(groupId, userId);
            await _groups.AddPayment(groupId, new Payment { Value = 0, UserId = userId, CurrencyType = CurrencyType.USD });
        }

        public async Task RejectInvitation(ObjectId userId, ObjectId groupId)
        {
            await _users.RejectInvitation(groupId, userId);
        }

        public async Task<ICollection<InvitationItemModel>> GetAllInvitations(ObjectId userId)
        {
           return (await _users.GetAllInvitations(userId))
                .Select(g => new InvitationItemModel {GroupId = g.Id, GroupName = g.Name}).ToList();
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
