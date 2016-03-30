using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Entities;

namespace MongoDB
{
    class DbUserRepository : IUserRepository
    {
        private IMongoCollection<Group> _groups;
        private IMongoCollection<UserGroupMembership> _memberships;
        public DbUserRepository(){
            var client = new MongoClient();
            var db = client.GetDatabase("balance");
            _groups = db.GetCollection<Group>(nameof(Group));
            _memberships = db.GetCollection<UserGroupMembership>(nameof(UserGroupMembership));
        }
        public async Task<ICollection<ObjectId>> GetAllGroupsOfUser(ObjectId userId)
        {
            return (await _memberships.FindAsync(m => m.UserId == userId)).ToList().Select(m => m.GroupId).ToList();
        }
    }
}
