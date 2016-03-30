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
        private IUserRepository _users = new DbUserRepository();


        public async Task AddGroup(AddGroupModel groupModel, ObjectId userId)
        {
            var groups = await _users.GetAllGroupsOfUser(userId);
            var id = new ObjectId();
            if (groups.Select(g => userId).Contains(id))
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
            await _groups.AddUserToGroup(Role.Administrator, userId, id);


        }



        public async Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId)
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



 




        public async Task<AddGroupModel> GetGroup(ObjectId id) { 
            var group = await _groups.GetGroup(id);
            if (group == null)
            {
                throw new Exception("Такой группы не существует");
            }

            return new AddGroupModel { Name = group.Name, Description = group.Description };
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
            var group = await _groups.GetGroup(groupId);
            //var users = await _users.GetAllUsers();
            if (group == null)
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
            var group = await _groups.GetGroup(groupId);
            var payments = await _groups.GetAllPayments(groupId);
            if (group == null)
            {
                throw new Exception("Такой группы не существует");
            }
            return payments.Select(p => new PaymentListItemModel { Id = p.UserId, Value = p.Value }).ToList();

        }

        public async Task<ICollection<UserListItemModel>> GetAllUsersInGroup(ObjectId groupId)
        {
            var users = await _groups.GetAllUsersInGroup(groupId);
            return users.Select(u => new UserListItemModel { Id = u }).ToList();
        }


        public async Task<ICollection<GroupListItemModel>> GetAllGroupsOfUser(ObjectId userId)
        {
            var groups = await _users.GetAllGroupsOfUser(userId);
            return groups.Select(g => new GroupListItemModel { Id = g.Id, Name = g.Name }).ToList();
        }
    }
}
