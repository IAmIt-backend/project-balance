using Entities;
using MongoDB;
using MongoDB.Bson;
using MVCModels.Models;
using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelAbstractions
{
    public class Service : IService
    {


        private IGroupRepository _groups = new DbGroupRepository();
        // private IUserRepository _users;


        public async Task AddGroup(AddGroupModel groupModel)
        {
            var groups = await _groups.GetAllGroups();
            var id = new ObjectId();
            if (groups.Select(g => g.Id).Contains(id))
            {
                throw new Exception("Такая группа уже существует");
            }
            var group = new Group
            {
                Id = id,
                Name = groupModel.Name,
                Description = groupModel.Description,
                Payments = new List<Payment>()
            };
            await _groups.AddGroup(group);


        }



        public async Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId)
        {
            if (await _groups.IsUserInGroup(userId, groupId) == true)
            {
                throw new Exception("Этот пользователь уже находится в группе");
            }
            else
            {
                await _groups.AddUserToGroup(memberType, userId, groupId);
            }

        }



        public async Task<ICollection<GroupListItemModel>> GetAllGroups()
        {
            var groups = await _groups.GetAllGroups();
            return groups.Select(g => new GroupListItemModel { Id = g.Id, Name = g.Name }).ToList();
        }



        public async Task<AddGroupModel> GetGroup(ObjectId id)
        {
            var groups = await _groups.GetAllGroups();
            if (!groups.Select(g => g.Id).Contains<ObjectId>(id))
            {
                throw new Exception("Такой группы не существует");
            }
            return groups.Select(g => new AddGroupModel { Name = g.Name, Description = g.Description }).First();
        }



        /*public async Task<User> GetUser(string email)
        {
            var users = await _users.GetAllUsers();
            if (email == null)
            {
                throw new Exception("Неверный email");
            }
            return user;


        }*/



        public async Task AddPayment(ObjectId groupId, decimal value, ObjectId userId)
        {
            var groups = await _groups.GetAllGroups();
            //var users = await _users.GetAllUsers();
            if (!groups.Select(g => g.Id).Contains<ObjectId>(groupId))
            {
                throw new Exception("Такой группы не существует");
            }
            else if (value <= 0)
            {
                throw new Exception("Нельзя внести отрицательную или нулевую сумму");
            }
            /* else if (!users.Contains<string>(email))
             {
                 throw new Exception("Такого пользователя не существует");
             }*/
            await _groups.AddPayment(groupId, new Payment { UserId = userId, Value = value });
        }


        public async Task<ICollection<PaymentListItemModel>> GetAllPayments(ObjectId groupId)
        {
            var groups = await _groups.GetAllGroups();
            var payments = await _groups.GetAllPayments(groupId);
            if (!groups.Select(g => g.Id).Contains<ObjectId>(groupId))
            {
                throw new Exception("Такой группы не существует");
            }
            return payments.Select(p => new PaymentListItemModel { Id = p.UserId, Value = p.Value }).ToList();

        }
    }
}
