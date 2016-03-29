using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDB
{
    public class DbGroupRepository : IGroupRepository
    {
        private IMongoCollection<Group> _groups;
        //private IMongoCollection<UserGroupMembership> _memberships;
        public DbGroupRepository()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("balance");
            _groups = db.GetCollection<Group>(nameof(Group));
            //_memberships = db.GetCollection<UserGroupMembership>(nameof(UserGroupMembership));    
        }
        public async Task AddGroup(Group group)
        {
            await Task.Run(() => { _groups.InsertOne(group); });
        }

        public Task AddUserToGroup(ObjectId userId, ObjectId groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Group>> GetAllGroups()
        {
            return await Task.Run(() =>
            {
                var list = _groups.AsQueryable().ToList();
                return list;
            }
            );
        }

        public Task<Group> GetGroup(ObjectId id)
        {
            //return await _groups.FindAsync(g => g.Id == id);
            throw new NotImplementedException();

        }
    }
}
