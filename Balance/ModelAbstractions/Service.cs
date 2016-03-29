using Balance.Models;
using Entities;
using MongoDB.Bson;
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


        private IGroupRepository _groups;
        private IUserRepository _users;


        public async Task AddGroup(AddGroupModel groupModel)
        {
            var groups = await _groups.GetAllGroups();
            var id = new ObjectId();
            if (groups.ToDictionary<ObjectId, string>(g => g.Id).Contains<ObjectId>(id))
            {
                throw new Exception("Такая группа уже существует");
            }
            var group = new Group
            {
                Id = id,
                Name = groupModel.Name,
                Description = groupModel.Description,
                Payments = new ICollection<Payment>()
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



        public async Task<IDictionary<ObjectId, string>> GetAllGroups()
        {
            var groups = await _groups.GetAllGroups();
            if (groups.Count.Equals(0))
            {
                throw new Exception("Ни одной группы не существует");
            }
            return groups.ToDictionary<ObjectId, string>(g => g.Id);

        }



        public async Task<Group> GetGroup(ObjectId id)
        {
            var groups = await _groups.GetAllGroups();
            if (!groups.ToDictionary<ObjectId, string>(g => g.Id).Contains<ObjectId>(id))
            {
                throw new Exception("Такой группы не существует");
            }
            return await groups.Where(g => g.Id == id).FirstAsync;
        }



        public async Task<User> GetUser(string email)
        {
            var users = await _users.GetAllUsers();
            if (email == null)
            {
                throw new Exception("Неверный email");
            }
            return user;


        }



        public async Task AddPayment(ObjectId groupId, double value, string email)
        {
            var groups = await _groups.GetAllGroups();
            var users = await _users.GetAllUsers();
            if (!groups.ToDictionary<ObjectId, string>(g => g.Id).Contains<ObjectId>(id))
            {
                throw new Exception("Такой группы не существует");
            }
            else if (value <= 0)
            {
                throw new Exception("Нельзя внести отрицательную или нулевую сумму");
            }
            else if (!users.Contains<string>(email))
            {
                throw new Exception("Такого пользователя не существует");
            }
            await _groups.AddPayment(groupId, value, GetUser(email).Id);
        }
    }
}
